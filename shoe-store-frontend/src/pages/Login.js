import { useState, useContext } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import AuthContext from '../contexts/AuthContext';
import { toast } from 'react-toastify';
import { getCurrentUser } from '../services/authService';
import "./index.css";

function Login() {
  const [credentials, setCredentials] = useState({ email: '', password: '' });
  const { signIn } = useContext(AuthContext);
  const navigate = useNavigate();

  const handleChange = (e) => {
    setCredentials({ ...credentials, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      await signIn(credentials);
      const user = await getCurrentUser();

      if (user.role === 'Admin') {
        navigate('/admin');
      } else {
        navigate('/');
      }
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
          <h2 className="register-title">Đăng nhập</h2>

          <form onSubmit={handleSubmit}>
            <input
              type="email"
              name="email"
              value={credentials.email}
              onChange={handleChange}
              placeholder="Email"
              className="register-input"
              required
            />

            <input
              type="password"
              name="password"
              value={credentials.password}
              onChange={handleChange}
              placeholder="Mật khẩu"
              className="register-input"
              required
            />

            <button type="submit" className="register-btn">Đăng nhập</button>
          </form>

          <div className="register-divider">Hoặc</div>

          <div className="social-login">
            <button className="btn-social facebook">Facebook</button>
            <button className="btn-social google">Google</button>
          </div>

          <p className="register-footer">
            Bạn chưa có tài khoản? <Link to="/register">Đăng ký</Link>
          </p>
        </div>
      </div>
    </div>
  );
}

export default Login;
