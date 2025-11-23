import { Routes, Route } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { CartProvider } from './contexts/CartContext';
import { PaymentProvider } from './contexts/PaymentContext'; // Import PaymentProvider
import { Elements } from '@stripe/react-stripe-js'; // Import Elements
import { loadStripe } from '@stripe/stripe-js'; // Import loadStripe
import Header from './components/Header';
import Footer from './components/Footer';
import Home from './pages/Home';
import Login from './pages/Login';
import Register from './pages/Register';
import Cart from './pages/Cart';
import Checkout from './pages/Checkout';
import Orders from './pages/Orders';
import Profile from './pages/Profile';
import AdminPanel from './Admin/AdminPanel';
import ProductDetail from './pages/ProductDetail';
import { ToastContainer } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import './index.css'; // Thay App.css bằng index.css

// Khởi tạo Stripe với publishable key
const stripePromise = loadStripe('pk_test_51S7eAQD6eIosWjRLv87daDBWniaLLB777tAxeLuCzoA8uJwsDTim1z619thIHRlZf1EGlrZm4XUxBVLj46Jne6KO001MMFd67f'); // Thay bằng key của bạn

function App() {
  return (
    <AuthProvider>
      <CartProvider>
        <PaymentProvider>
          <Elements stripe={stripePromise}>
            <div className="app-container">
              <Header />
              <main>
                <Routes>
                  <Route path="/" element={<Home />} />
                  <Route path="/login" element={<Login />} />
                  <Route path="/register" element={<Register />} />
                  <Route path="/cart" element={<Cart />} />
                  <Route path="/checkout" element={<Checkout />} />
                  <Route path="/orders" element={<Orders />} />
                  <Route path="/profile" element={<Profile />} />
                  <Route path="/admin" element={<AdminPanel />} />
                  <Route path="/productdetail" element={<ProductDetail />} />
                  <Route path="/products/:id" element={<ProductDetail />} />
                </Routes>
              </main>
              <Footer />
              <ToastContainer position="top-right" autoClose={3000} hideProgressBar />
            </div>
          </Elements>
        </PaymentProvider>
      </CartProvider>
    </AuthProvider>
  );
}

export default App;