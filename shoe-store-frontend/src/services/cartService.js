// (Giữ nguyên code từ user)
import axios from "axios";
import { getToken } from "./authService";

const API_BASE_URL = "http://localhost:5172/api/cart";

// Lấy giỏ hàng
export async function getCart() {
  const token = getToken();
  const res = await axios.get(API_BASE_URL, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

// Thêm vào giỏ
export async function addToCart(item) {
  const token = getToken();
  const res = await axios.post(API_BASE_URL, item, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

// Cập nhật số lượng
export async function updateCart(productId, quantity) {
  const token = getToken();
  const res = await axios.put(`${API_BASE_URL}/${productId}`, quantity, {
    headers: {
      Authorization: `Bearer ${token}`,
      "Content-Type": "application/json"
    }
  });
  return res.data;
}

// Xoá khỏi giỏ
export async function removeFromCart(productId) {
  const token = getToken();
  const res = await axios.delete(`${API_BASE_URL}/${productId}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}