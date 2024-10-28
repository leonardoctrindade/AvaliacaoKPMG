// src/__tests__/AuthContext.test.js
import React from 'react';
import { render, screen } from '@testing-library/react';
import { AuthProvider, useAuth } from '../contexts/AuthContext';

const TestComponent = () => {
    const { user, login, logout } = useAuth();
    return (
        <div>
            <span>{user ? 'Logged in' : 'Logged out'}</span>
            <button onClick={() => login('test@example.com', 'password123')}>Login</button>
            <button onClick={logout}>Logout</button>
        </div>
    );
};

describe('AuthContext', () => {
    test('provides user state and login/logout functions', () => {
        render(
            <AuthProvider>
                <TestComponent />
            </AuthProvider>
        );

        expect(screen.getByText(/logged out/i)).toBeInTheDocument();

        // Simular login
        fireEvent.click(screen.getByText(/login/i));
        expect(screen.getByText(/logged in/i)).toBeInTheDocument();

        // Simular logout
        fireEvent.click(screen.getByText(/logout/i));
        expect(screen.getByText(/logged out /i)).toBeInTheDocument();
    });
});
