import React, { createContext, useContext, useState } from 'react';

const PaymentContext = createContext();

export function PaymentProvider({ children }) {
  const [paymentStatus, setPaymentStatus] = useState(null); // Giữ lại nếu cần sử dụng sau
  const [error, setError] = useState(null); // Giữ lại để Checkout có thể set error

  const value = {
    paymentStatus,
    error,
    setError, // Cho phép Checkout set error
  };

  return <PaymentContext.Provider value={value}>{children}</PaymentContext.Provider>;
}

export const usePayment = () => useContext(PaymentContext);