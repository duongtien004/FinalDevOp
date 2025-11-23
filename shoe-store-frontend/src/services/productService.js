import axios from "axios";

const API_BASE_URL = "http://localhost:5172/api/products";

// Lấy danh sách sản phẩm (có thể tìm kiếm)
export async function getProducts(search) {
  try {
    const res = await axios.get(API_BASE_URL, {
      params: { search: search || "" },
    });
    return res.data;
  } catch (err) {
    console.error("Lỗi khi lấy sản phẩm:", err);
    return [];
  }
}

// Lấy chi tiết sản phẩm
export async function getProductById(id) {
  try {
    const res = await axios.get(`${API_BASE_URL}/${id}`);
    return res.data;
  } catch (err) {
    console.error("Lỗi khi lấy sản phẩm:", err);
    return null;
  }
}

// Tạo sản phẩm mới (Admin)
export async function createProduct(product, token) {
  const res = await axios.post(API_BASE_URL, product, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return res.data;
}

// Cập nhật sản phẩm (Admin)
export async function updateProduct(id, product, token) {
  const res = await axios.put(`${API_BASE_URL}/${id}`, product, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return res.data;
}

// Xóa sản phẩm (Admin)
export async function deleteProduct(id, token) {
  const res = await axios.delete(`${API_BASE_URL}/${id}`, {
    headers: { Authorization: `Bearer ${token}` },
  });
  return res.data;
}
