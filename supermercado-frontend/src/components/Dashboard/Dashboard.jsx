import React from 'react';
import { ShoppingBag, Users, DollarSign, TrendingUp } from 'lucide-react';

const Dashboard = () => {
  const stats = [
    {
      title: 'Total de Produtos',
      value: '126',
      change: '+12.5%',
      icon: ShoppingBag,
      color: 'bg-indigo-600'
    },
    {
      title: 'Vendas do Mês',
      value: 'R$ 45.678',
      change: '+23.1%',
      icon: DollarSign,
      color: 'bg-green-500'
    },
    {
      title: 'Novos Clientes',
      value: '48',
      change: '+4.2%',
      icon: Users,
      color: 'bg-blue-500'
    },
    {
      title: 'Taxa de Crescimento',
      value: '15.3%',
      change: '+2.3%',
      icon: TrendingUp,
      color: 'bg-purple-500'
    }
  ];

  return (
    <div>
      <h1 className="text-2xl font-semibold text-gray-900">Dashboard</h1>
      
      {/* Stats Grid */}
      <div className="mt-6 grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {stats.map((item, index) => (
          <div
            key={index}
            className="bg-white overflow-hidden shadow rounded-lg"
          >
            <div className="p-5">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <div className={`p-3 rounded-lg ${item.color}`}>
                    <item.icon className="h-6 w-6 text-white" />
                  </div>
                </div>
                <div className="ml-5 w-0 flex-1">
                  <dl>
                    <dt className="text-sm font-medium text-gray-500 truncate">
                      {item.title}
                    </dt>
                    <dd className="flex items-baseline">
                      <div className="text-2xl font-semibold text-gray-900">
                        {item.value}
                      </div>
                      <div className="ml-2 flex items-baseline text-sm font-semibold text-green-600">
                        {item.change}
                      </div>
                    </dd>
                  </dl>
                </div>
              </div>
            </div>
          </div>
        ))}
      </div>

      {/* Recent Activity */}
      <div className="mt-8">
        <div className="bg-white shadow rounded-lg p-6">
          <h2 className="text-lg font-medium text-gray-900 mb-4">
            Atividades Recentes
          </h2>
          {/* Add your activity content here */}
        </div>
      </div>
    </div>
  );
};

export default Dashboard;