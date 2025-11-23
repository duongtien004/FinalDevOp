import { useState, useContext } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthContext from '../contexts/AuthContext';
import { toast } from 'react-toastify';
import "./index.css"; // Tạo file riêng cho trang này

function Register() {
  const [userData, setUserData] = useState({ username: '', email: '', password: '' });
  const { signUp } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setUserData({ ...userData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await signUp(userData);
      navigate('/login');
    } catch (err) {
      toast.error(err.message);
    }
  };

  return (
    <div className="register-page">
      {/* Bên trái: logo + slogan */}
      <div className="register-left">
        <img src="/123.png" alt="Logo" className="register-logo" /> 
        {/* <h1 className="brand-name">ShoeStore</h1>
        <p className="brand-slogan">Nền tảng mua sắm giày uy tín hàng đầu</p> */}
      </div>

      {/* Bên phải: form */}
      <div className="register-right">
        <div className="register-card">
          <h2 className="register-title">Đăng ký</h2>

          <form onSubmit={handleSubmit}>
            <input
              type="text"
              name="username"
              value={userData.username}
              onChange={handleChange}
              placeholder="Tên đăng nhập"
              className="register-input"
              required
            />

            <input
              type="email"
              name="email"
              value={userData.email}
              onChange={handleChange}
              placeholder="Email"
              className="register-input"
              required
            />

            <input
              type="password"
              name="password"
              value={userData.password}
              onChange={handleChange}
              placeholder="Mật khẩu"
              className="register-input"
              required
            />

            <button type="submit" className="register-btn">Đăng ký</button>
          </form>

          <div className="register-divider">Hoặc</div>

          <div className="social-login">
            <button className="btn-social facebook">Facebook</button>
            <button className="btn-social google">Google</button>
          </div>

          <p className="register-footer">
            Bạn đã có tài khoản? <Link to="/login">Đăng nhập</Link>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Register;
