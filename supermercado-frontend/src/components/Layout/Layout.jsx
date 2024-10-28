import React, { useState, useEffect } from 'react';
import api from '../../services/api/api';
import { Plus, Edit, Trash2, Eye } from 'lucide-react';

const ProdutoList = () => {
  const [produtos, setProdutos] = useState([]);
  const [showModal, setShowModal] = useState(false);
  const [selectedProduto, setSelectedProduto] = useState(null);
  const [modalMode, setModalMode] = useState('add');
  const [formData, setFormData] = useState({
    nome: '',
    setor: '',
    descricao: '',
    preco: ''
  });

  useEffect(() => {
    loadProdutos();
  }, []);

  const loadProdutos = async () => {
    try {
      const response = await api.get('/produto');
      setProdutos(response.data);
    } catch (error) {
      console.error('Erro ao carregar produtos:', error);
      alert('Erro ao carregar produtos');
    }
  };

  const handleInputChange = (e) => {
    const { name, value } = e.target;
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));
  };

  const openModal = (mode, produto = null) => {
    setModalMode(mode);
    if (produto && (mode === 'edit' || mode === 'view')) {
      setSelectedProduto(produto);
      setFormData({
        nome: produto.nome,
        setor: produto.setor,
        descricao: produto.descricao,
        preco: produto.preco
      });
    } else {
      setSelectedProduto(null);
      setFormData({
        nome: '',
        setor: '',
        descricao: '',
        preco: ''
      });
    }
    setShowModal(true);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (modalMode === 'edit') {
        await api.put(`/produto/${selectedProduto.id}`, {
          id: selectedProduto.id,
          ...formData
        });
      } else if (modalMode === 'add') {
        await api.post('/produto', formData);
      }
      setShowModal(false);
      loadProdutos();
    } catch (error) {
      console.error('Erro ao salvar produto:', error);
      alert('Erro ao salvar produto');
    }
  };

  const handleDelete = async (id) => {
    if (window.confirm('Tem certeza que deseja excluir este produto?')) {
      try {
        await api.delete(`/produto/${id}`);
        loadProdutos();
      } catch (error) {
        console.error('Erro ao excluir produto:', error);
        alert('Erro ao excluir produto');
      }
    }
  };

  return (
    <div className="container mx-auto px-4">
      {/* Header */}
      <div className="flex justify-between items-center mb-6">
        <div>
          <h1 className="text-2xl font-bold text-gray-800">Produtos</h1>
          <p className="text-gray-600">Total de produtos: {produtos.length}</p>
        </div>
        <button
          onClick={() => openModal('add')}
          className="flex items-center bg-black hover:bg-gray-800 text-white px-6 py-2 rounded"
        >
          <Plus size={20} className="mr-2" />
          Novo Produto
        </button>
      </div>

      {/* Tabela */}
      <div className="bg-white rounded-lg shadow overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-100">
              <tr>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Nome</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Setor</th>
                <th className="px-6 py-3 text-left text-sm font-semibold text-gray-900">Descrição</th>
                <th className="px-6 py-3 text-right text-sm font-semibold text-gray-900">Preço</th>
                <th className="px-6 py-3 text-center text-sm font-semibold text-gray-900">Ações</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {produtos.map((produto) => (
                <tr key={produto.id} className="hover:bg-gray-50">
                  <td className="px-6 py-4 text-sm text-gray-900">{produto.nome}</td>
                  <td className="px-6 py-4 text-sm text-gray-600">{produto.setor}</td>
                  <td className="px-6 py-4 text-sm text-gray-600">{produto.descricao}</td>
                  <td className="px-6 py-4 text-sm text-gray-900 text-right">
                    {parseFloat(produto.preco).toLocaleString('pt-BR', {
                      style: 'currency',
                      currency: 'BRL'
                    })}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-center">
                    <div className="flex justify-center gap-2">
                      <button
                        onClick={() => openModal('view', produto)}
                        className="bg-black hover:bg-gray-800 text-white px-3 py-1 rounded text-sm inline-flex items-center"
                      >
                        <Eye size={16} className="mr-1" />
                        Ver
                      </button>
                      <button
                        onClick={() => openModal('edit', produto)}
                        className="bg-black hover:bg-gray-800 text-white px-3 py-1 rounded text-sm inline-flex items-center"
                      >
                        <Edit size={16} className="mr-1" />
                        Editar
                      </button>
                      <button
                        onClick={() => handleDelete(produto.id)}
                        className="bg-black hover:bg-gray-800 text-white px-3 py-1 rounded text-sm inline-flex items-center"
                      >
                        <Trash2 size={16} className="mr-1" />
                        Excluir
                      </button>
                    </div>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Modal */}
      {showModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4 z-50">
          <div className="bg-white rounded-lg shadow-xl max-w-md w-full">
            <div className="px-6 py-4 border-b border-gray-200">
              <h3 className="text-xl font-semibold text-gray-800">
                {modalMode === 'add' && 'Novo Produto'}
                {modalMode === 'edit' && 'Editar Produto'}
                {modalMode === 'view' && 'Detalhes do Produto'}
              </h3>
            </div>
            <form onSubmit={handleSubmit}>
              <div className="p-6 space-y-4">
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Nome</label>
                  <input
                    type="text"
                    name="nome"
                    value={formData.nome}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-black"
                    required
                    disabled={modalMode === 'view'}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Setor</label>
                  <input
                    type="text"
                    name="setor"
                    value={formData.setor}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-black"
                    required
                    disabled={modalMode === 'view'}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Descrição</label>
                  <textarea
                    name="descricao"
                    value={formData.descricao}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-black"
                    rows="3"
                    disabled={modalMode === 'view'}
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-gray-700 mb-1">Preço</label>
                  <input
                    type="number"
                    name="preco"
                    value={formData.preco}
                    onChange={handleInputChange}
                    className="w-full px-3 py-2 border border-gray-300 rounded focus:outline-none focus:ring-1 focus:ring-black"
                    step="0.01"
                    required
                    disabled={modalMode === 'view'}
                  />
                </div>
              </div>
              <div className="px-6 py-4 border-t border-gray-200 flex justify-end gap-3">
                <button
                  type="button"
                  onClick={() => setShowModal(false)}
                  className="px-4 py-2 text-sm font-medium text-white bg-black hover:bg-gray-800 rounded"
                >
                  {modalMode === 'view' ? 'Fechar' : 'Cancelar'}
                </button>
                {modalMode !== 'view' && (
                  <button
                    type="submit"
                    className="px-4 py-2 text-sm font-medium text-white bg-black hover:bg-gray-800 rounded"
                  >
                    {modalMode === 'add' ? 'Adicionar' : 'Salvar'}
                  </button>
                )}
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default ProdutoList;