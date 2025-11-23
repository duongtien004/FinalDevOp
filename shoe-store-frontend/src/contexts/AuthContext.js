import { createContext, useState, useEffect } from "react";
import { login, register, getCurrentUser, setToken, getToken } from "../services/authService";
import { updateUser } from "../services/userService";
import { toast } from "react-toastify";

const AuthContext = createContext();

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);
  const [isAdmin, setIsAdmin] = useState(false);

  useEffect(() => {
    const token = getToken();
    if (token) {
      getCurrentUser()
        .then((data) => {
          setUser(data);
          setIsAdmin(data.role === "Admin");
        })
        .catch((err) => {
          toast.error(err.message);
          setToken(null);
        })
        .finally(() => setLoading(false));
    } else {
      setLoading(false);
    }
  }, []);

  const signIn = async (credentials) => {
    try {
      const data = await login(credentials);
      if (data.token) {
        setToken(data.token);
        const userData = await getCurrentUser();
        setUser(userData);
        setIsAdmin(userData.role === "Admin");
        toast.success("Logged in successfully!");
        return userData;
      }
      throw new Error("Login failed");
    } catch (err) {
      toast.error(err.message);
      throw err;
    }
  };

  const signUp = async (userData) => {
    try {
      const data = await register(userData);
      toast.success("Registration successful! Please sign in.");
      return data;
    } catch (err) {
      toast.error(err.message);
      throw err;
    }
  };

  const signOut = () => {
    setToken(null);
    setUser(null);
    setIsAdmin(false);
    toast.info("Logged out successfully.");
  };

  const updateProfile = async (updatedData) => {
    try {
      const data = await updateUser(user.id, {
        username: updatedData.username,
        email: updatedData.email,
        role: user.role,
      });
      setUser(data);
      setIsAdmin(data.role === "Admin");
      toast.success("Profile updated successfully!");
      return data;
    } catch (err) {
      toast.error(err.message);
      throw err;
    }
  };

  return (
    <AuthContext.Provider value={{ user, loading, isAdmin, signIn, signUp, signOut, updateProfile }}>
      {!loading && children}
    </AuthContext.Provider>
  );
}

export default AuthContext;