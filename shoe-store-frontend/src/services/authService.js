import axios from "axios";

const API_BASE_URL = "http://localhost:5172/api/auth";

// Lưu token vào localStorage
export function setToken(token) {
  localStorage.setItem("token", token);
}

// Lấy token
export function getToken() {
  return localStorage.getItem("token");
}

// Đăng ký
export async function register(userData) {
  const res = await axios.post(`${API_BASE_URL}/register`, userData);
  return res.data;
}

// Đăng nhập
export async function login(credentials) {
  const res = await axios.post(`${API_BASE_URL}/login`, credentials, {
    headers: { 'Content-Type': 'application/json' }
  });
  if (res.data.token) {
    setToken(res.data.token);
  }
  return res.data; // Trả về { token }
}

// Lấy thông tin user hiện tại
export async function getCurrentUser() {
  const token = getToken();
  if (!token) throw new Error('No token available');
  const res = await axios.get(`${API_BASE_URL}/me`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}