import { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { useElements, useStripe, CardElement } from '@stripe/react-stripe-js';
import { usePayment } from '../contexts/PaymentContext';
import { useCart } from '../contexts/CartContext'; // Sử dụng useCart thay vì CartContext
import { placeOrder, confirmPayment } from '../services/orderService';
import { toast } from 'react-toastify';

function Checkout() {
  const { cart, total, fetchCart, clearCart } = useCart();
  const { loading: paymentLoading, error: paymentError } = usePayment();
  const stripe = useStripe();
  const elements = useElements();
  const navigate = useNavigate();

  const [formData, setFormData] = useState({
    shippingAddress: '',
    paymentMethod: 'CreditCard',
  });

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!cart.length) {
      toast.error('Giỏ hàng trống. Vui lòng thêm sản phẩm trước khi đặt hàng.');
      return;
    }

    const orderData = {
      items: cart.map((item) => ({
        productId: item.productId,
        quantity: item.quantity,
        price: item.product.price,
      })),
      totalAmount: total,
      shippingAddress: formData.shippingAddress,
      paymentMethod: formData.paymentMethod,
    };
    console.log('OrderData gửi đi:', orderData);

    try {
      let orderResponse;
      if (formData.paymentMethod === 'CreditCard' && stripe && elements) {
        // Bước 1: Đặt hàng và nhận clientSecret
        orderResponse = await placeOrder(orderData);
        if (!orderResponse.clientSecret) {
          throw new Error('Không nhận được thông tin thanh toán từ server.');
        }

        // Bước 2: Xác nhận thanh toán với Stripe
        const cardElement = elements.getElement(CardElement);
        if (!cardElement) throw new Error('Không thể tải form thanh toán.');
        
        const { error, paymentIntent } = await stripe.confirmCardPayment(
          orderResponse.clientSecret,
          {
            payment_method: { card: cardElement },
            billing_details: { address: { line1: formData.shippingAddress } },
          }
        );

        if (error) throw new Error(error.message);
        if (paymentIntent.status !== 'succeeded') {
          throw new Error('Thanh toán không thành công. Vui lòng thử lại.');
        }

        // Bước 3: Cập nhật trạng thái thanh toán trên BE
        await confirmPayment(orderResponse.id, paymentIntent.id);
        orderResponse.status = 'Paid'; // Cập nhật trạng thái cục bộ
      } else {
        orderResponse = await placeOrder(orderData);
        if (orderResponse.status !== 'Processing') {
          throw new Error('Đặt hàng không thành công');
        }
      }

      clearCart();
      fetchCart();
      toast.success('Đặt hàng và thanh toán thành công!');
      navigate('/orders');
    } catch (err) {
      console.error('Lỗi khi đặt hàng:', err);
      toast.error(paymentError || err.message || 'Lỗi khi đặt hàng');
    }
  };

  return (
    <div className="checkout-wrapper container py-12">
      <h2 className="checkout-title">THANH TOÁN</h2>
      <div className="checkout-content">
        <table className="checkout-table">
          <thead>
            <tr>
              <th>Hình ảnh</th>
              <th>Tên sản phẩm</th>
              <th>Số lượng</th>
              <th>Đơn giá</th>
              <th>Tổng</th>
            </tr>
          </thead>
          <tbody>
            {cart.map((item) => (
              <tr key={item.productId}>
                <td>
                  <img
                    src={item.product.imageUrl || 'placeholder.jpg'}
                    alt={item.product.name}
                    className="checkout-img"
                  />
                </td>
                <td className="product-name">{item.product.name}</td>
                <td>{item.quantity}</td>
                <td>{item.product.price.toLocaleString()}₫</td>
                <td>{(item.product.price * item.quantity).toLocaleString()}₫</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="checkout-summary">
          <h3>Thông tin đơn hàng</h3>
          <form onSubmit={handleSubmit} className="checkout-form">
            <div className="form-group">
              <label>Địa chỉ giao hàng</label>
              <input
                type="text"
                name="shippingAddress"
                value={formData.shippingAddress}
                onChange={handleChange}
                required
                placeholder="Nhập địa chỉ giao hàng"
              />
            </div>
            <div className="form-group">
              <label>Phương thức thanh toán</label>
              <select
                name="paymentMethod"
                value={formData.paymentMethod}
                onChange={handleChange}
                required
              >
                <option value="CreditCard">Credit Card</option>
                <option value="PayPal">PayPal</option>
                <option value="CashOnDelivery">Cash on Delivery</option>
              </select>
            </div>
            <div className="summary-box">
              <p>
                Thành tiền: <span>{total.toLocaleString()}₫</span>
              </p>
              <p className="summary-total">
                Tổng cộng: <span>{total.toLocaleString()}₫</span>
              </p>
            </div>
            {formData.paymentMethod === 'CreditCard' && (
              <div className="form-group">
                <label>Thẻ tín dụng</label>
                <CardElement className="stripe-element" />
              </div>
            )}
            <button
              type="submit"
              disabled={(formData.paymentMethod === 'CreditCard' && (!stripe || paymentLoading)) || !formData.shippingAddress}
              className="checkout-btn"
            >
              {paymentLoading ? 'Đang xử lý...' : 'ĐẶT HÀNG'}
            </button>
            {paymentError && <p className="text-center text-red-500 mt-2">{paymentError}</p>}
          </form>
        </div>
      </div>
    </div>
  );
}

export default Checkout;