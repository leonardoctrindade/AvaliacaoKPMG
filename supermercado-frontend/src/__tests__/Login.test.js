// src/__tests__/Login.test.js
import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import { AuthProvider } from '../contexts/AuthContext';
import Login from '../components/Login';
import { useAuth } from '../contexts/AuthContext';

// Mock do hook useAuth
jest.mock('../contexts/AuthContext');

describe('Login Component', () => {
    beforeEach(() => {
        useAuth.mockReturnValue({
            login: jest.fn().mockResolvedValue(true),
            loading: false,
            error: null
        });
    });

    test('renders login form', () => {
        render(
            <AuthProvider>
                <Login />
            </AuthProvider>
        );

        expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
        expect(screen.getByLabelText(/senha/i)).toBeInTheDocument();
        expect(screen.getByRole('button', { name: /entrar/i })).toBeInTheDocument();
    });

    test('submits form with email and password', async () => {
        render(
            <AuthProvider>
                <Login />
            </AuthProvider>
        );

        fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'test@example.com' } });
        fireEvent.change(screen.getByLabelText(/senha/i), { target: { value: 'password123' } });
        fireEvent.click(screen.getByRole('button', { name: /entrar/i }));

        expect(useAuth().login).toHaveBeenCalledWith('test@example.com', 'password123');
    });

    test('displays error message on login failure', async () => {
        useAuth.mockReturnValue({
            login: jest.fn().mockResolvedValue(false),
            loading: false,
            error: 'Erro ao fazer login'
        });

        render(
            <AuthProvider>
                <Login />
            </AuthProvider>
        );

        fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'test@example.com' } });
        fireEvent.change(screen.getByLabelText(/senha/i), { target: { value: 'password123' } });
        fireEvent.click(screen.getByRole('button', { name: /entrar/i }));

        expect(await screen.findByText(/erro ao fazer login/i)).toBeInTheDocument();
    });
});