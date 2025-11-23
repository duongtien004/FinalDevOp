import { useEffect, useState, useContext } from "react";
import { Link, useLocation } from "react-router-dom"; // ✅ chỉ import 1 lần
import { getProducts } from "../services/productService";
import CartContext from "../contexts/CartContext";
import "./index.css";

function Home() {
  const [products, setProducts] = useState([]);
  const { addItemToCart } = useContext(CartContext);
  const location = useLocation();

  useEffect(() => {
    async function fetchData() {
      const params = new URLSearchParams(location.search);
      const search = params.get("search") || "";
      const data = await getProducts(search); // ✅ gọi API kèm search
      setProducts(data);
    }
    fetchData();
  }, [location.search]); // ✅ reload khi search thay đổi

  return (
    <>
      {/* Hero Banner */}
      <section className="hero"></section>

      {/* Danh sách sản phẩm */}
      <section id="products" className="container">
        <h2 className="section-title">Sản phẩm nổi bật</h2>

        <div className="product-grid">
          {products.length > 0 ? (
            products.map((p) => (
              <div key={p.id} className="product-card">
                <div className="product-image">
                  {/* Ảnh sản phẩm -> link sang ProductDetail */}
                  <Link to={`/products/${p.id}`}>
                    <img src={p.imageUrl} alt={p.name} />
                  </Link>

                  {/* Hover overlay thêm giỏ hàng */}
                  <button
                    className="btn-overlay"
                    onClick={() =>
                      addItemToCart({ productId: p.id, quantity: 1 })
                    }
                  >
                    🛒 Thêm vào giỏ hàng
                  </button>
                </div>

                {/* Thông tin sản phẩm */}
                <div className="product-info">
                  <Link to={`/products/${p.id}`} className="product-name">
                    {p.name}
                  </Link>
                  <p className="price">{p.price.toLocaleString()} đ</p>
                  {p.oldPrice && (
                    <p className="old-price">
                      {p.oldPrice.toLocaleString()} đ
                    </p>
                  )}
                </div>
              </div>
            ))
          ) : (
            <p className="no-products">Không có sản phẩm nào.</p>
          )}
        </div>
      </section>
    </>
  );
}

export default Home;
