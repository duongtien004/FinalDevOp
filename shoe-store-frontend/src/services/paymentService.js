import { loadStripe } from '@stripe/stripe-js';
import axios from 'axios';
import { getToken } from './authService';

export const stripePromise = loadStripe(''); // Publishable Key

export async function getClientSecret(paymentMethod, totalAmount, orderItems, shippingAddress) {
  try {
    const token = getToken();
    const response = await axios.post(
      'http://backend:8088/api/orders',
      {
        paymentMethod,
        totalAmount,
        items: orderItems,
        shippingAddress,
      },
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );
    if (!response.data.clientSecret) {
      throw new Error('Không nhận được clientSecret từ server');
    }
    return response.data.clientSecret;
  } catch (error) {
    throw new Error(error.response?.data?.message || 'Lỗi khi khởi tạo thanh toán');
  }
}

// Xóa confirmPayment vì đã xử lý trong Checkout