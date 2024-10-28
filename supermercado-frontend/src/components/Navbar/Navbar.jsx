import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../../contexts/AuthContext';

const Navbar = () => {
  const { user, logout } = useAuth();

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark">
      <div className="container">
        <Link className="navbar-brand" to="/">Supermercado</Link>
        <div className="navbar-nav">
          {user ? (
            <>
              <Link className="nav-link" to="/produtos">Produtos</Link>
              <button className="nav-link btn btn-link" onClick={logout}>Sair</button>
            </>
          ) : (
            <Link className="nav-link" to="/login">Login</Link>
          )}
        </div>
      </div>
    </nav>
  );
};

export default Navbar;