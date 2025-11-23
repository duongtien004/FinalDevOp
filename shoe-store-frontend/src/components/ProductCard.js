import { useContext } from 'react';
import CartContext from '../contexts/CartContext';
import { toast } from 'react-toastify';

function ProductCard({ product }) {
  const { addItemToCart } = useContext(CartContext);

  const handleAddToCart = async () => {
    try {
      await addItemToCart({ productId: product.id, quantity: 1 });
    } catch (err) {
      toast.error(err.message);
    }
  };

  return (
    <div className="bg-white rounded-xl shadow-md overflow-hidden transition-all duration-300 hover:shadow-xl hover:-translate-y-1">
      <img
        src={product.imageUrl || 'placeholder.jpg'}
        alt={product.name}
        className="w-full h-48 object-cover transition-transform duration-300 hover:scale-105"
      />
      <div className="p-5">
        <h3 className="text-lg font-semibold text-gray-800 truncate">{product.name}</h3>
        <p className="text-gray-500 text-sm mt-1 line-clamp-2">{product.description}</p>
        <p className="text-gray-800 font-bold text-lg mt-2">${product.price.toFixed(2)}</p>
        <button
          onClick={handleAddToCart}
          className="mt-4 w-full btn-primary"
        >
          Add to Cart
        </button>
      </div>
    </div>
  );
}

export default ProductCard;