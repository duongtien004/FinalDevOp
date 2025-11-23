import { useState, useEffect, useContext } from "react";
import {
  getAllUsers,
  createUser,
  updateUser,
  deleteUser,
} from "../services/userService";
import {
  getProducts,
  createProduct,
  updateProduct,
  deleteProduct,
} from "../services/productService";
import {
  getAllComments,
  createComment,
  deleteComment,
} from "../services/commentService"; // Đảm bảo import đúng
import AuthContext from "../contexts/AuthContext";
import { toast } from "react-toastify";
import "./admin.css";

function AdminPanel() {
  const { isAdmin } = useContext(AuthContext);

  // ===== Users =====
  const [users, setUsers] = useState([]);
  const [newUser, setNewUser] = useState({
    username: "",
    email: "",
    password: "",
    role: "User",
  });
  const [editingUser, setEditingUser] = useState(null);

  // ===== Products =====
  const [products, setProducts] = useState([]);
  const [newProduct, setNewProduct] = useState({
    name: "",
    description: "",
    price: "",
    imageUrl: "",
    category: "",
  });
  const [editingProduct, setEditingProduct] = useState(null);

  // ===== Comments =====
  const [comments, setComments] = useState([]);
  const [newComment, setNewComment] = useState({
    productId: "", // Thêm productId để liên kết comment với sản phẩm
    content: "",
  });
  const [editingComment, setEditingComment] = useState(null);

  // ===== Loading & Error =====
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  // ====================== Fetch Data ======================
  useEffect(() => {
    if (isAdmin) {
      fetchUsers();
      fetchProducts();
      fetchComments();
    }
  }, [isAdmin]);

  const fetchUsers = async () => {
    setLoading(true);
    try {
      const data = await getAllUsers();
      setUsers(data);
    } catch (err) {
      setError(err.message);
      toast.error(err.message);
    } finally {
      setLoading(false);
    }
  };

  const fetchProducts = async () => {
    try {
      const data = await getProducts();
      setProducts(data);
    } catch (err) {
      toast.error("Failed to load products");
    }
  };

  const fetchComments = async () => {
    try {
      const data = await getAllComments();
      setComments(data);
    } catch (err) {
      toast.error("Failed to load comments");
    }
  };

  // ====================== Users ======================
  const handleCreateUser = async (e) => {
    e.preventDefault();
    if (!newUser.username || !newUser.email || !newUser.password) {
      toast.error("Please fill all required fields");
      return;
    }
    try {
      const token = localStorage.getItem("token");
      await createUser(newUser, token);
      setNewUser({ username: "", email: "", password: "", role: "User" });
      fetchUsers();
      toast.success("User created successfully");
    } catch (err) {
      console.error("Create user error:", err.response?.data || err.message);
      toast.error(err.response?.data?.message || "Failed to create user");
    }
  };

  const handleUpdateUser = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      const payload = { ...editingUser };
      if (!payload.password) {
        delete payload.password;
      }
      await updateUser(editingUser.id, payload, token);
      setEditingUser(null);
      fetchUsers();
      toast.success("User updated successfully");
    } catch (err) {
      console.error("Update user error:", err.response?.data || err.message);
      toast.error(err.response?.data?.message || "Failed to update user");
    }
  };

  const handleDeleteUser = async (id) => {
    if (window.confirm("Are you sure you want to delete this user?")) {
      try {
        const token = localStorage.getItem("token");
        await deleteUser(id, token);
        fetchUsers();
        toast.success("User deleted successfully");
      } catch (err) {
        console.error("Delete user error:", err.response?.data || err.message);
        toast.error(err.response?.data?.message || "Failed to delete user");
      }
    }
  };

  // ====================== Products ======================
  const handleCreateProduct = async (e) => {
    e.preventDefault();
    if (!newProduct.name || !newProduct.price || !newProduct.imageUrl) {
      toast.error("Please fill all required fields");
      return;
    }
    try {
      const token = localStorage.getItem("token");
      const payload = {
        ...newProduct,
        price: Number(newProduct.price),
      };
      await createProduct(payload, token);
      setNewProduct({
        name: "",
        description: "",
        price: "",
        imageUrl: "",
        category: "",
      });
      fetchProducts();
      toast.success("Product created successfully");
    } catch (err) {
      console.error("Create product error:", err.response?.data || err.message);
      toast.error(err.response?.data?.message || "Failed to create product");
    }
  };

  const handleUpdateProduct = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem("token");
      const payload = {
        ...editingProduct,
        price: Number(editingProduct.price),
      };
      await updateProduct(editingProduct.id, payload, token);
      setEditingProduct(null);
      fetchProducts();
      toast.success("Product updated successfully");
    } catch (err) {
      console.error("Update product error:", err.response?.data || err.message);
      toast.error(err.response?.data?.message || "Failed to update product");
    }
  };

  const handleDeleteProduct = async (id) => {
    if (window.confirm("Are you sure you want to delete this product?")) {
      try {
        const token = localStorage.getItem("token");
        await deleteProduct(id, token);
        fetchProducts();
        toast.success("Product deleted successfully");
      } catch (err) {
        toast.error("Failed to delete product");
      }
    }
  };

  // ====================== Comments ======================
  const handleCreateComment = async (e) => {
    e.preventDefault();
    if (!newComment.productId || !newComment.content) {
      toast.error("Please select a product and fill the comment content");
      return;
    }
    try {
      const token = localStorage.getItem("token");
      const comment = await createComment(newComment.productId, newComment.content);
      setComments([...comments, comment]);
      setNewComment({ productId: "", content: "" });
      fetchComments(); // Cập nhật danh sách
      toast.success("Comment created successfully");
    } catch (err) {
      console.error("Create comment error:", err.response?.data || err.message);
      toast.error(err.response?.data?.message || "Failed to create comment");
    }
  };

  const handleDeleteComment = async (id) => {
    if (window.confirm("Are you sure you want to delete this comment?")) {
      try {
        const token = localStorage.getItem("token");
        await deleteComment(id, token);
        setComments(comments.filter((c) => c.id !== id));
        fetchComments(); // Cập nhật danh sách
        toast.success("Comment deleted successfully");
      } catch (err) {
        console.error("Delete comment error:", err.response?.data || err.message);
        toast.error(err.response?.data?.message || "Failed to delete comment");
      }
    }
  };

  const handleRefreshComments = async () => {
    try {
      await fetchComments();
      toast.success("Comments refreshed successfully");
    } catch (err) {
      toast.error("Failed to refresh comments");
    }
  };

  if (!isAdmin) {
    return (
      <div className="admin-container">
        <p className="error-msg">Access denied. Admin only.</p>
      </div>
    );
  }

  return (
    <div className="admin-layout">
      {/* Sidebar */}
      <aside className="sidebar">
        <h2 className="sidebar-title">Admin Panel</h2>
        <nav className="sidebar-menu">
          <a href="#dashboard" className="active">
            Dashboard
          </a>
          <a href="#users">Users</a>
          <a href="#products">Products</a>
          <a href="#comments">Comments</a>
          <a href="#orders">Orders</a>
        </nav>
      </aside>

      {/* Main */}
      <main className="admin-main">
        <h2 className="admin-title">Admin Dashboard</h2>
        {error && <p className="error-msg">{error}</p>}

        {/* Users */}
        <section id="users" className="card">
          <h3 className="card-title">Manage Users</h3>
          <form onSubmit={handleCreateUser} className="form">
            <input
              type="text"
              name="username"
              placeholder="Username"
              value={newUser.username}
              onChange={(e) => setNewUser({ ...newUser, username: e.target.value })}
              required
            />
            <input
              type="email"
              name="email"
              placeholder="Email"
              value={newUser.email}
              onChange={(e) => setNewUser({ ...newUser, email: e.target.value })}
              required
            />
            <input
              type="password"
              name="password"
              placeholder="Password"
              value={newUser.password}
              onChange={(e) => setNewUser({ ...newUser, password: e.target.value })}
              required
            />
            <select
              name="role"
              value={newUser.role}
              onChange={(e) => setNewUser({ ...newUser, role: e.target.value })}
            >
              <option value="User">User</option>
              <option value="Admin">Admin</option>
            </select>
            <button type="submit" className="btn btn-primary">
              Create User
            </button>
          </form>

          {editingUser && (
            <div className="modal">
              <div className="modal-content">
                <h3>Edit User</h3>
                <form onSubmit={handleUpdateUser} className="form">
                  <input
                    type="text"
                    value={editingUser.username}
                    onChange={(e) =>
                      setEditingUser({ ...editingUser, username: e.target.value })
                    }
                    required
                  />
                  <input
                    type="email"
                    value={editingUser.email}
                    onChange={(e) =>
                      setEditingUser({ ...editingUser, email: e.target.value })
                    }
                    required
                  />
                  <input
                    type="password"
                    placeholder="New Password (optional)"
                    value={editingUser.password || ""}
                    onChange={(e) =>
                      setEditingUser({ ...editingUser, password: e.target.value })
                    }
                  />
                  <select
                    value={editingUser.role}
                    onChange={(e) =>
                      setEditingUser({ ...editingUser, role: e.target.value })
                    }
                  >
                    <option value="User">User</option>
                    <option value="Admin">Admin</option>
                  </select>
                  <div className="modal-actions">
                    <button
                      type="button"
                      onClick={() => setEditingUser(null)}
                      className="btn btn-cancel"
                    >
                      Cancel
                    </button>
                    <button type="submit" className="btn btn-primary">
                      Update
                    </button>
                  </div>
                </form>
              </div>
            </div>
          )}

          <div className="table-wrapper">
            {loading ? (
              <div className="loader"></div>
            ) : (
              <table className="admin-table">
                <thead>
                  <tr>
                    <th>ID</th>
                    <th>Username</th>
                    <th>Email</th>
                    <th>Role</th>
                    <th>Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {users.map((u) => (
                    <tr key={u.id}>
                      <td>{u.id}</td>
                      <td>{u.username}</td>
                      <td>{u.email}</td>
                      <td className={`role ${u.role.toLowerCase()}`}>{u.role}</td>
                      <td>
                        <button
                          onClick={() => setEditingUser(u)}
                          className="btn btn-edit"
                        >
                          Edit
                        </button>
                        <button
                          onClick={() => handleDeleteUser(u.id)}
                          className="btn btn-delete"
                        >
                          Delete
                        </button>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            )}
          </div>
        </section>

        {/* Products */}
        <section id="products" className="card">
          <h3 className="card-title">Manage Products</h3>
          <form onSubmit={handleCreateProduct} className="form">
            <input
              type="text"
              placeholder="Product name"
              value={newProduct.name}
              onChange={(e) =>
                setNewProduct({ ...newProduct, name: e.target.value })
              }
              required
            />
            <textarea
              placeholder="Description"
              value={newProduct.description}
              onChange={(e) =>
                setNewProduct({ ...newProduct, description: e.target.value })
              }
            />
            <input
              type="number"
              placeholder="Price"
              value={newProduct.price}
              onChange={(e) =>
                setNewProduct({ ...newProduct, price: e.target.value })
              }
              required
            />
            <input
              type="text"
              placeholder="Image URL"
              value={newProduct.imageUrl}
              onChange={(e) =>
                setNewProduct({ ...newProduct, imageUrl: e.target.value })
              }
              required
            />
            <input
              type="text"
              placeholder="Category"
              value={newProduct.category}
              onChange={(e) =>
                setNewProduct({ ...newProduct, category: e.target.value })
              }
            />
            <button type="submit" className="btn btn-primary">
              Create Product
            </button>
          </form>

          <div className="table-wrapper">
            <table className="admin-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Name</th>
                  <th>Price</th>
                  <th>Category</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {products.map((p) => (
                  <tr key={p.id}>
                    <td>{p.id}</td>
                    <td>{p.name}</td>
                    <td>{p.price.toLocaleString()} đ</td>
                    <td>{p.category}</td>
                    <td>
                      <button
                        onClick={() => setEditingProduct(p)}
                        className="btn btn-edit"
                      >
                        Edit
                      </button>
                      <button
                        onClick={() => handleDeleteProduct(p.id)}
                        className="btn btn-delete"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>

        {/* Edit Product Modal */}
        {editingProduct && (
          <div className="modal">
            <div className="modal-content">
              <h3>Edit Product</h3>
              <form onSubmit={handleUpdateProduct} className="form">
                <input
                  type="text"
                  value={editingProduct.name}
                  onChange={(e) =>
                    setEditingProduct({ ...editingProduct, name: e.target.value })
                  }
                  required
                />
                <textarea
                  value={editingProduct.description}
                  onChange={(e) =>
                    setEditingProduct({
                      ...editingProduct,
                      description: e.target.value,
                    })
                  }
                />
                <input
                  type="number"
                  value={editingProduct.price}
                  onChange={(e) =>
                    setEditingProduct({
                      ...editingProduct,
                      price: e.target.value,
                    })
                  }
                  required
                />
                <input
                  type="text"
                  value={editingProduct.imageUrl}
                  onChange={(e) =>
                    setEditingProduct({
                      ...editingProduct,
                      imageUrl: e.target.value,
                    })
                  }
                  required
                />
                <input
                  type="text"
                  value={editingProduct.category}
                  onChange={(e) =>
                    setEditingProduct({
                      ...editingProduct,
                      category: e.target.value,
                    })
                  }
                />
                <div className="modal-actions">
                  <button
                    type="button"
                    onClick={() => setEditingProduct(null)}
                    className="btn btn-cancel"
                  >
                    Cancel
                  </button>
                  <button type="submit" className="btn btn-primary">
                    Update
                  </button>
                </div>
              </form>
            </div>
          </div>
        )}

        {/* Comments */}
        <section id="comments" className="card">
          <h3 className="card-title">Manage Comments</h3>
          <button
            onClick={handleRefreshComments}
            className="btn btn-refresh"
            style={{ marginBottom: "10px" }}
          >
            Refresh Comments
          </button>
          <form onSubmit={handleCreateComment} className="form">
            <select
              value={newComment.productId}
              onChange={(e) =>
                setNewComment({ ...newComment, productId: e.target.value })
              }
              required
            >
              <option value="">Select Product</option>
              {products.map((p) => (
                <option key={p.id} value={p.id}>
                  {p.name}
                </option>
              ))}
            </select>
            <input
              type="text"
              placeholder="Comment content"
              value={newComment.content}
              onChange={(e) =>
                setNewComment({ ...newComment, content: e.target.value })
              }
              required
            />
            <button type="submit" className="btn btn-primary">
              Create Comment
            </button>
          </form>

          <div className="table-wrapper">
            <table className="admin-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Product</th>
                  <th>Username</th>
                  <th>Content</th>
                  <th>Created At</th>
                  <th>Actions</th>
                </tr>
              </thead>
              <tbody>
                {comments.map((c) => (
                  <tr key={c.id}>
                    <td>{c.id}</td>
                    <td>{c.productName}</td>
                    <td>{c.username}</td>
                    <td>{c.content}</td>
                    <td>{new Date(c.createdAt).toLocaleString()}</td>
                    <td>
                      <button
                        onClick={() => handleDeleteComment(c.id)}
                        className="btn btn-delete"
                      >
                        Delete
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </section>
      </main>
    </div>
  );
}

export default AdminPanel;