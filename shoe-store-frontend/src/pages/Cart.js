import { useContext } from "react";
import { Link } from "react-router-dom";
import CartContext from "../contexts/CartContext";
import "./index.css";

function Cart() {
  const { cart, updateItemQuantity, removeItemFromCart, total } =
    useContext(CartContext);

  return (
    <div className="cart-wrapper container py-12">
      <h2 className="cart-title">GIỎ HÀNG CỦA BẠN</h2>

      {cart.length === 0 ? (
        <p className="empty-cart">
          Giỏ hàng trống.{" "}
          <Link to="/" className="shop-link">
            Mua sắm ngay!
          </Link>
        </p>
      ) : (
        <div className="cart-content">
          {/* Bảng giỏ hàng */}
          <table className="cart-table">
            <thead>
              <tr>
                <th>Hình ảnh</th>
                <th>Tên sản phẩm</th>
                <th>Mã hàng</th>
                <th>Số lượng</th>
                <th className="col-price">Đơn giá</th>
                <th className="col-price">Tổng cộng</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {cart.map((item) => {
                if (!item.product) return null;
                return (
                  <tr key={item.productId}>
                    <td>
                      <img
                        src={item.product.imageUrl || "placeholder.jpg"}
                        alt={item.product.name}
                        className="cart-img"
                      />
                    </td>
                    <td className="text-left">
                      <p className="product-name">{item.product.name}</p>
                    </td>
                    <td>{item.product.code || "-"}</td>
                    <td>
                      <div className="quantity-control">
                        <button
                          onClick={() =>
                            updateItemQuantity(item.productId, item.quantity - 1)
                          }
                          disabled={item.quantity <= 1}
                        >
                          -
                        </button>
                        <input type="text" value={item.quantity} readOnly />
                        <button
                          onClick={() =>
                            updateItemQuantity(item.productId, item.quantity + 1)
                          }
                        >
                          +
                        </button>
                      </div>
                    </td>
                    <td className="col-price">
                      {item.product.price.toLocaleString()}₫
                    </td>
                    <td className="col-price">
                      {(item.product.price * item.quantity).toLocaleString()}₫
                    </td>
                    <td>
                      <button
                        className="remove-btn"
                        onClick={() => removeItemFromCart(item.productId)}
                      >
                        ×
                      </button>
                    </td>
                  </tr>
                );
              })}
            </tbody>
          </table>

          {/* Khối tổng tiền */}
          <div className="cart-summary">
            <h3>Sử dụng mã giảm giá</h3>
            <div className="summary-box">
              <p>
                Thành tiền: <span>{(total + 55000).toLocaleString()}₫</span>
              </p>
              <p>
                Tặng tất miễn phí: <span>-55.000₫</span>
              </p>
              <p className="summary-total">
                Tổng: <span>{total.toLocaleString()}₫</span>
              </p>
            </div>
            <Link to="/checkout" className="checkout-btn">
              THANH TOÁN
            </Link>
          </div>
        </div>
      )}
    </div>
  );
}

export default Cart;
