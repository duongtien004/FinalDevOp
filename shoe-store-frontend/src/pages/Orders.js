import { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import { getUserOrders } from "../services/orderService";
import { toast } from "react-toastify";
import "./index.css";

function Orders() {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = async () => {
      try {
        const data = await getUserOrders();
        setOrders(data);
      } catch (err) {
        toast.error("Không thể tải đơn hàng");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, []);

  const getStatusText = (st) => {
    if (st === "Delivered") return "Đã giao";
    if (st === "Processing") return "Đang xử lý";
    if (st === "Paid") return "Đã thanh toán";
    return "Đã hủy";
  };

  const getStatusClass = (st) => {
    if (st === "Delivered") return "order-status done";
    if (st === "Processing") return "order-status processing";
    if (st === "Paid") return "order-status paid";
    return "order-status cancel";
  };

  return (
    <div className="cart-wrapper container py-12">
      <h2 className="cart-title">ĐƠN HÀNG CỦA BẠN</h2>

      {loading ? (
        <p className="empty-cart">Đang tải...</p>
      ) : orders.length === 0 ? (
        <p className="empty-cart">
          Bạn chưa có đơn hàng nào —{" "}
          <Link to="/" className="shop-link">
            Mua ngay!
          </Link>
        </p>
      ) : (
        <div className="cart-content">
          {/* BẢNG ĐƠN HÀNG */}
          <table className="cart-table">
            <thead>
              <tr>
                <th>Mã đơn</th>
                <th>Ngày đặt</th>
                <th>Trạng thái</th>
                <th>Tổng tiền</th>
                <th>Phương thức</th>
                <th>Sản phẩm</th>
              </tr>
            </thead>

            <tbody>
              {orders.map((o) => (
                <tr key={o.id}>
                  <td># {o.id}</td>

                  <td>
                    {new Date(o.orderDate).toLocaleDateString("vi-VN")}
                  </td>

                  <td>
                    <span className={getStatusClass(o.status)}>
                      {getStatusText(o.status)}
                    </span>
                  </td>

                  <td className="col-price">
                    {o.totalAmount.toLocaleString("vi-VN")}₫
                  </td>

                  <td>{o.paymentMethod}</td>

                  <td className="text-left">
                    {(o.items || []).map((item, idx) => (
                      <div
                        key={idx}
                        style={{
                          borderBottom: "1px solid #eee",
                          padding: "4px 0",
                        }}
                      >
                        {item.productName} × {item.quantity}
                      </div>
                    ))}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}

export default Orders;
