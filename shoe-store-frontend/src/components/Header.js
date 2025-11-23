import { Link } from "react-router-dom";
import { useContext, useState } from "react";
import AuthContext from "../contexts/AuthContext";
import CartContext from "../contexts/CartContext"; // 🔥 Thêm
import { ShoppingCart, Menu, X } from "lucide-react";
import { useNavigate } from "react-router-dom";
import "./index.css";

function Header() {
  const [search, setSearch] = useState('');
  const navigate = useNavigate();
  const { user, isAdmin, signOut } = useContext(AuthContext);
  const { cart } = useContext(CartContext); // 🔥 Lấy giỏ hàng từ context
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  // Tính tổng số lượng
  const cartCount = cart.reduce((sum, item) => sum + item.quantity, 0);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };
  
    const handleSearch = (e) => {
    if (e.key === "Enter") {
      e.preventDefault();
      navigate(`/?search=${encodeURIComponent(search)}`);
    }
  };

  return (
    <header className="header">
      {/* Top bar */}
      <div className="top-bar">Hotline: 0123 456 789</div>

      {/* Main header */}
      <div className="header-container">
        {/* Logo */}
        <Link to="/" className="logo">
          <img src="/111.jpg" alt="" />
          <span>ShoeStore</span>
        </Link>

        {/* Desktop Navigation */}
        <nav className="nav-menu">
          <Link to="/nike">GIÀY NIKE</Link>
          <Link to="/adidas">GIÀY ADIDAS</Link>
          <Link to="/mlb">GIÀY MLB</Link>
          <Link to="/phu-kien">PHỤ KIỆN</Link>
          <Link to="/tin-tuc">TIN TỨC</Link>
          <Link to="/lien-he">LIÊN HỆ</Link>
          {user && (
            <>
              <Link to="/profile">Profile</Link>
              <Link to="/orders">Orders</Link>
              {isAdmin && <Link to="/admin">Admin</Link>}
            </>
          )}
        </nav>

        {/* Right side */}
        <div className="header-right">
          {/* Search */}
      <input
        type="text"
        value={search}
        onChange={(e) => setSearch(e.target.value)}
        onKeyDown={handleSearch} // 👈 enter để search
        placeholder="Search for shoes..."
        className="input-field"
        style={{ maxWidth: '400px', width: '100%' }}
      />

{/* Auth */}
{user ? (
  <button onClick={signOut} className="btn-auth logout">
    Đăng xuất
  </button>
) : (
  <div className="auth-buttons">
    <Link to="/login" className="btn-auth login">Đăng nhập</Link>
    <Link to="/register" className="btn-auth register">Đăng ký</Link>
  </div>
)}


          {/* Cart */}
          <Link to="/cart" className="cart-icon">
            <ShoppingCart size={22} />
            <span className="cart-count">{cartCount}</span>
          </Link>

          {/* Mobile menu button */}
          <button className="menu-btn" onClick={toggleMenu} aria-label="Toggle menu">
            {isMenuOpen ? <X size={24} /> : <Menu size={24} />}
          </button>
        </div>
      </div>

      {/* Mobile Navigation */}
      {isMenuOpen && (
        <nav className="mobile-menu">
          <Link to="/nike" onClick={toggleMenu}>GIÀY NIKE</Link>
          <Link to="/adidas" onClick={toggleMenu}>GIÀY ADIDAS</Link>
          <Link to="/mlb" onClick={toggleMenu}>GIÀY MLB</Link>
          <Link to="/phu-kien" onClick={toggleMenu}>PHỤ KIỆN</Link>
          <Link to="/tin-tuc" onClick={toggleMenu}>TIN TỨC</Link>
          <Link to="/lien-he" onClick={toggleMenu}>LIÊN HỆ</Link>
          {user ? (
            <>
              <Link to="/profile" onClick={toggleMenu}>Profile</Link>
              <Link to="/orders" onClick={toggleMenu}>Orders</Link>
              {isAdmin && <Link to="/admin" onClick={toggleMenu}>Admin</Link>}
              <button
                onClick={() => {
                  signOut();
                  toggleMenu();
                }}
                className="logout-btn"
              >
                Logout
              </button>
            </>
          ) : (
            <>
              <Link to="/login" onClick={toggleMenu}>Đăng nhập</Link>
              <Link to="/register" onClick={toggleMenu}>Đăng ký</Link>
            </>
          )}
        </nav>
      )}
    </header>
  );
}

export default Header;
