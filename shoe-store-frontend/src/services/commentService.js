import axios from "axios";
import { getToken } from "./authService";

const API_URL = "http://backend:8088/api/comments";

// Lấy danh sách comment theo productId
export const getComments = async (productId) => {
  try {
    const res = await axios.get(`${API_URL}/product/${productId}`, {
      headers: { Authorization: `Bearer ${getToken()}` },
    });
    return res.data;
  } catch (error) {
    console.error("Get comments error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Không thể lấy danh sách bình luận.");
  }
};

// Tạo comment mới (yêu cầu token)
export const createComment = async (productId, content) => {
  const token = getToken();
  if (!token) throw new Error("Token không tồn tại. Vui lòng đăng nhập lại.");

  try {
    const res = await axios.post(
      `${API_URL}/product/${productId}`, // Sửa endpoint
      { content },
      { headers: { Authorization: `Bearer ${token}` } }
    );
    return res.data;
  } catch (error) {
    console.error("Create comment error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Không thể tạo bình luận.");
  }
};

// Lấy chi tiết comment theo id
export const getCommentById = async (id) => {
  try {
    const res = await axios.get(`${API_URL}/${id}`, {
      headers: { Authorization: `Bearer ${getToken()}` },
    });
    return res.data;
  } catch (error) {
    console.error("Get comment by id error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Không thể lấy bình luận.");
  }
};

// Cập nhật comment theo id (yêu cầu token)
export const updateComment = async (id, content) => {
  const token = getToken();
  if (!token) throw new Error("Token không tồn tại. Vui lòng đăng nhập lại.");

  try {
    const res = await axios.put(
      `${API_URL}/${id}`,
      { content },
      { headers: { Authorization: `Bearer ${token}` } }
    );
    return res.data;
  } catch (error) {
    console.error("Update comment error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Không thể cập nhật bình luận.");
  }
};

// Xóa comment theo id (yêu cầu token)
export const deleteComment = async (id) => {
  const token = getToken();
  if (!token) throw new Error("Token không tồn tại. Vui lòng đăng nhập lại.");

  try {
    await axios.delete(`${API_URL}/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
  } catch (error) {
    console.error("Delete comment error:", error.response?.data || error.message);
    throw new Error(error.response?.data?.message || "Không thể xóa bình luận.");
  }
};

// Lấy tất cả comment (chỉ Admin)
export const getAllComments = async () => {
  const token = getToken();
  if (!token) throw new Error("Token không tồn tại. Vui lòng đăng nhập lại.");

  try {
    const res = await axios.get(`${API_URL}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return res.data;
  } catch (error) {
    console.error("Get all comments error:", error.response?.data || error.message);
    if (error.response?.status === 404) {
      console.warn("Endpoint GET all comments not found. Returning empty array.");
      return [];
    }
    throw new Error(error.response?.data?.message || "Không thể lấy tất cả bình luận.");
  }
};