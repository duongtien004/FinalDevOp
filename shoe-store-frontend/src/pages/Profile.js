import { useState, useContext, useEffect } from "react";
import { Link } from "react-router-dom";
import { FaUser, FaEnvelope } from "react-icons/fa";
import AuthContext from "../contexts/AuthContext";
import { toast } from "react-toastify";
import "./index.css";

function Profile() {
  const { user, updateProfile } = useContext(AuthContext);
  const [formData, setFormData] = useState({ username: "", email: "" });

  useEffect(() => {
    if (user) {
      setFormData({ username: user.username || "", email: user.email || "" });
    }
  }, [user]);

  const handleChange = (e) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await updateProfile(formData);
      toast.success("Cập nhật thành công!");
    } catch (err) {
      toast.error(err.message);
    }
  };

  if (!user) {
    return (
      <div className="profile-not-login">
        <p>
          Bạn cần đăng nhập — <Link to="/login">Đăng nhập</Link>
        </p>
      </div>
    );
  }

  return (
    <section className="profile-container">
      <h2 className="profile-title">Thông Tin Cá Nhân</h2>

      <div className="profile-box">
        <form onSubmit={handleSubmit}>
          <div className="profile-field">
            <label>
              <FaUser className="profile-icon" /> Tên người dùng
            </label>
            <input
              type="text"
              name="username"
              value={formData.username}
              onChange={handleChange}
              required
            />
          </div>

          <div className="profile-field">
            <label>
              <FaEnvelope className="profile-icon" /> Email
            </label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
            />
          </div>

          <button type="submit" className="profile-btn">
            Lưu thay đổi
          </button>
        </form>
      </div>
    </section>
  );
}

export default Profile;
