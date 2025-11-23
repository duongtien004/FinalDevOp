import { createContext, useState, useEffect, useContext } from "react";
import { getCart, addToCart, updateCart, removeFromCart } from "../services/cartService";
import AuthContext from "./AuthContext";
import { toast } from "react-toastify";

const CartContext = createContext();

export function CartProvider({ children }) {
  const [cart, setCart] = useState([]);
  const { user } = useContext(AuthContext);

  useEffect(() => {
    if (user) {
      fetchCart();
    } else {
      setCart([]);
    }
  }, [user]);

  const fetchCart = async () => {
    try {
      const data = await getCart();
      console.log("Cart data from API:", data); // Debug dữ liệu API
      // Lọc bỏ các item không có product
      const validCart = data.filter(item => item.product != null);
      if (validCart.length < data.length) {
        toast.warn("Some cart items were removed due to invalid products.");
      }
      setCart(validCart);
    } catch (err) {
      toast.error(err.message);
      setCart([]);
    }
  };

  const addItemToCart = async (item) => {
    try {
      await addToCart(item);
      fetchCart();
      toast.success("Item added to cart!");
    } catch (err) {
      toast.error(err.message);
    }
  };

  const updateItemQuantity = async (productId, quantity) => {
    if (quantity < 1) return;
    try {
      await updateCart(productId, quantity);
      fetchCart();
      toast.success("Cart updated!");
    } catch (err) {
      toast.error(err.message);
    }
  };

  const removeItemFromCart = async (productId) => {
    try {
      await removeFromCart(productId);
      fetchCart();
      toast.success("Item removed from cart!");
    } catch (err) {
      toast.error(err.message);
    }
  };

  const total = cart.reduce((sum, item) => sum + (item.product?.price || 0) * item.quantity, 0);

  return (
    <CartContext.Provider
      value={{
        cart,
        addItemToCart,
        updateItemQuantity,
        removeItemFromCart,
        total,
        fetchCart
      }}
    >
      {children}
    </CartContext.Provider>
  );
}

// ✅ Thêm dòng này để có thể import { useCart } ở các file khác
export const useCart = () => useContext(CartContext);

export default CartContext;
