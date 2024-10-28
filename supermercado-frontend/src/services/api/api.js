import axios from 'axios';

const api = axios.create({
  baseURL: 'http://localhost:5077/api', // Note o uso de http ao invés de https
  headers: {
    'Content-Type': 'application/json'
  }
});

// Interceptor para adicionar token em todas as requisições
api.interceptors.request.use(
  config => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    console.log('Request Config:', config); // Log para debug
    return config;
  },
  error => {
    console.error('Request Error:', error); // Log para debug
    return Promise.reject(error);
  }
);

// Interceptor para tratar erros de resposta
api.interceptors.response.use(
  response => response,
  error => {
    console.error('Response Error:', error); // Log para debug
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default api;