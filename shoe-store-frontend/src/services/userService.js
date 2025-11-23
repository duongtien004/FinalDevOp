import axios from "axios";
import { getToken } from "./authService"; // Hàm lấy token lưu ở localStorage

const API_BASE_URL = "http://localhost:5172/api/user";

export async function getAllUsers() {
  const token = getToken();
  const res = await axios.get(API_BASE_URL, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

export async function getUserById(id) {
  const token = getToken();
  const res = await axios.get(`${API_BASE_URL}/${id}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

export async function createUser(userDto) {
  const token = getToken();
  const res = await axios.post(API_BASE_URL, userDto, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

export async function updateUser(id, userDto) {
  const token = getToken();
  const res = await axios.put(`${API_BASE_URL}/${id}`, userDto, {
    headers: { Authorization: `Bearer ${token}` }
  });
  return res.data;
}

export async function deleteUser(id) {
  const token = getToken();
  await axios.delete(`${API_BASE_URL}/${id}`, {
    headers: { Authorization: `Bearer ${token}` }
  });
}
