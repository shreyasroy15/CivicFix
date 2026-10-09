import { useAuth } from '../contexts/AuthContext';
import { useNavigate } from 'react-router-dom';

export const Dashboard = () => {
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  return (
    <div className="min-h-screen bg-gray-100 p-8">
      <div className="max-w-4xl mx-auto bg-white shadow rounded-lg p-6">
        <div className="flex justify-between items-center mb-6">
          <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
          <button 
            onClick={handleLogout}
            className="px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
          >
            Logout
          </button>
        </div>
        <div className="bg-gray-50 p-4 rounded border">
          <h2 className="text-lg font-semibold mb-2">Welcome, {user?.username}!</h2>
          <p className="text-gray-600 mb-1">Email: {user?.email}</p>
          <p className="text-gray-600">Roles: {user?.roles.join(', ') || 'USER'}</p>
        </div>
      </div>
    </div>
  );
};
