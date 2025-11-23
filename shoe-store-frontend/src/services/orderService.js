import axios from 'axios';
import { getToken } from './authService';

const API_BASE_URL = 'http://localhost:5172/api/orders';

export async function placeOrder(orderData) {
  const token = getToken();
  try {
    const res = await axios.post(API_BASE_URL, orderData, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
  } catch (err) {
    throw new Error(err.response?.data?.message || 'Không thể đặt hàng. Vui lòng thử lại.');
  }
}

export async function confirmPayment(orderId, paymentIntentId) {
  const token = getToken();
  try {
    const res = await axios.post(`${API_BASE_URL}/${orderId}/confirm`, { paymentIntentId }, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
  } catch (err) {
    throw new Error(err.response?.data?.message || 'Không thể xác nhận thanh toán.');
  }
}

export async function getUserOrders() {
  const token = getToken();
  try {
    const res = await axios.get(API_BASE_URL, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
  } catch (err) {
    throw new Error(err.response?.data?.message || 'Không thể lấy danh sách đơn hàng.');
  }
}