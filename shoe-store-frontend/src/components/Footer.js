import React from "react";
import "./index.css";

function Footer() {
  return (
    <footer className="footer">
      <div className="footer-top">
        <div className="footer-col logo-col">
          <h2 className="footer-logo">ShoeStore</h2>
          <p className="footer-desc">
            Chuyên cung cấp các loại giày Nike, Adidas, MLB và phụ kiện thời trang.
            Uy tín - Chất lượng - Giá tốt.
          </p>
        </div>

        <div className="footer-col">
          <h3>Danh mục</h3>
          <ul>
            <li><a href="/nike">Giày Nike</a></li>
            <li><a href="/adidas">Giày Adidas</a></li>
            <li><a href="/mlb">Giày MLB</a></li>
            <li><a href="/phu-kien">Phụ kiện</a></li>
          </ul>
        </div>

        <div className="footer-col">
          <h3>Hỗ trợ</h3>
          <ul>
            <li><a href="/huong-dan">Hướng dẫn mua hàng</a></li>
            <li><a href="/chinh-sach">Chính sách đổi trả</a></li>
            <li><a href="/bao-hanh">Chính sách bảo hành</a></li>
            <li><a href="/lien-he">Liên hệ</a></li>
          </ul>
        </div>

        <div className="footer-col">
          <h3>Liên hệ</h3>
          <p>Hotline: 0123 456 789</p>
          <p>Email: support@shoestore.com</p>
          <div className="socials">
            <a href="#"><i className="fab fa-facebook"></i></a>
            <a href="#"><i className="fab fa-instagram"></i></a>
            <a href="#"><i className="fab fa-twitter"></i></a>
          </div>
        </div>
      </div>

      <div className="footer-bottom">
        <p>© 2025 ShoeStore. All rights reserved.</p>
      </div>
    </footer>
  );
}

export default Footer;
