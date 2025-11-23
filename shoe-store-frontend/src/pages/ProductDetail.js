import { useEffect, useState, useContext } from "react";
import { useParams, useNavigate } from "react-router-dom";
import { getProductById } from "../services/productService";
import { getComments, createComment, deleteComment } from "../services/commentService";
import CartContext from "../contexts/CartContext";
import { toast } from "react-toastify";
import "./index.css";

function ProductDetail() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [product, setProduct] = useState(null);
  const [quantity, setQuantity] = useState(1);
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState("");
  const { addItemToCart } = useContext(CartContext);

  useEffect(() => {
    async function fetchData() {
      try {
        const data = await getProductById(id);
        setProduct(data);
        const commentData = await getComments(id);
        setComments(commentData);
      } catch (error) {
        toast.error("Error loading product!");
      }
    }
    fetchData();
  }, [id]);

  const handleAddToCart = () => {
    addItemToCart({ productId: product.id, quantity });
    toast.success(`${product.name} added to cart!`);
  };

  const handleAddComment = async (e) => {
    e.preventDefault();
    if (!newComment.trim()) {
      toast.error("Vui lòng nhập nội dung bình luận!");
      return;
    }
    try {
      const token = localStorage.getItem("token"); // Sử dụng localStorage trực tiếp nếu không có authService
      if (!token) {
        toast.error("Bạn cần đăng nhập để bình luận!");
        return;
      }
      const comment = await createComment(id, newComment);
      setComments([...comments, comment]);
      setNewComment("");
      toast.success("Bình luận đã được tạo thành công!");
    } catch (err) {
      toast.error(err.message || "Không thể tạo bình luận. Vui lòng thử lại!");
    }
  };

  const handleDeleteComment = async (commentId) => {
    try {
      const token = localStorage.getItem("token"); // Sử dụng localStorage trực tiếp
      if (!token) {
        toast.error("Bạn cần đăng nhập để xóa bình luận!");
        return;
      }
      await deleteComment(commentId);
      setComments(comments.filter((c) => c.id !== commentId));
      toast.success("Bình luận đã được xóa thành công!");
    } catch (err) {
      toast.error(err.message || "Không thể xóa bình luận. Vui lòng thử lại!");
    }
  };

  if (!product) {
    return (
      <div className="product-loading">
        <p>Loading product details...</p>
      </div>
    );
  }

  return (
    <section className="product-detail-container">
      <div className="product-content">
        {/* Hình ảnh */}
        <div className="product-image-box">
          <img
            src={product.imageUrl}
            alt={product.name}
            className="product-image"
          />
        </div>

        {/* Thông tin */}
        <div className="product-info">
          <h1 className="product-title">{product.name}</h1>

          {/* Đánh giá */}
          <div className="product-rating">
            ⭐ {product.rating || 4.5} / 5 
            <span className="rating-count">
              ({product.reviewCount || 20} đánh giá)
            </span>
          </div>

          <p className="product-description">{product.description}</p>

          {/* Giá */}
          <div className="product-price-box">
            <p className="product-price">{product.price.toLocaleString()} đ</p>
            {product.oldPrice && (
              <p className="product-old-price">{product.oldPrice.toLocaleString()} đ</p>
            )}
            {product.discount && (
              <span className="product-discount">-{product.discount}%</span>
            )}
          </div>

          {/* Thông tin thêm */}
          <div className="product-extra">
            <p><strong>Màu sắc:</strong> {product.color || "Trắng, Đen, Xám"}</p>
            <p><strong>Size:</strong> {product.size || "36 - 43"}</p>
            <p><strong>Tình trạng:</strong> 
              {product.stock > 0 ? "Còn hàng ✅" : "Còn hàng ✅"}
            </p>
            <p><strong>Vận chuyển:</strong> Miễn phí cho đơn trên 500k 🚚</p>
          </div>

          {/* Số lượng */}
          <div className="product-quantity">
            <label htmlFor="quantity">Số lượng:</label>
            <input
              id="quantity"
              type="number"
              min="1"
              value={quantity}
              onChange={(e) => setQuantity(Number(e.target.value))}
            />
          </div>

          {/* Nút */}
          <div className="product-actions">
            <button onClick={handleAddToCart} className="btn-add">
              🛒 Thêm vào giỏ hàng
            </button>
            <button onClick={() => navigate(-1)} className="btn-view">
              Quay lại
            </button>
            <button className="btn-favorite">❤️ Yêu thích</button>
          </div>
        </div>
      </div>

      {/* Comment Section */}
      <div className="comment-box">
        <h3 className="comment-title">Bình luận</h3>

        {/* Form comment */}
        <form onSubmit={handleAddComment} className="comment-form">
          <input
            type="text"
            placeholder="Nhập bình luận..."
            value={newComment}
            onChange={(e) => setNewComment(e.target.value)}
          />
          <button type="submit">Gửi</button>
        </form>

        {/* Danh sách comment */}
        <div className="comment-list">
          {comments.length === 0 && (
            <p className="comment-empty">Chưa có bình luận nào.</p>
          )}
          {comments.map((c) => (
            <div key={c.id} className="comment-item">
              <div>
                <p className="comment-username">{c.username}</p>
                <p>{c.content}</p>
                <p className="comment-time">
                  {new Date(c.createdAt).toLocaleString()}
                </p>
              </div>
              <button
                onClick={() => handleDeleteComment(c.id)}
                className="comment-delete"
              >
                Xóa
              </button>
            </div>
          ))}
        </div>
      </div>
    </section>
  );
}

export default ProductDetail;