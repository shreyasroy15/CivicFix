import { useQuery } from '@tanstack/react-query';
import { adminApi } from '../../api/adminApi';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, PieChart, Pie, Cell } from 'recharts';
import { AlertCircle, CheckCircle, Clock, ShieldAlert, FileText, CheckSquare, RefreshCw, XCircle } from 'lucide-react';
import { Link } from 'react-router-dom';
import { format } from 'date-fns';

const COLORS = ['#0088FE', '#00C49F', '#FFBB28', '#FF8042', '#8884d8', '#ffc658'];

export const AdminDashboard = () => {
  const { data, isLoading, isError, refetch } = useQuery({
    queryKey: ['admin-dashboard'],
    queryFn: () => adminApi.getDashboardStats(),
  });

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (isError || !data) {
    return (
      <div className="text-center py-12">
        <p className="text-red-600 mb-4">Failed to load dashboard statistics.</p>
        <button onClick={() => refetch()} className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700">
          Retry
        </button>
      </div>
    );
  }

  const categoryData = Object.entries(data.issuesByCategory).map(([name, value]) => ({ name, value }));
  
  const StatCard = ({ title, value, icon: Icon, colorClass, subtitle }: any) => (
    <div className="bg-white rounded-lg shadow p-6 border-l-4 border-l-blue-500">
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium text-gray-500">{title}</p>
          <p className="text-3xl font-bold text-gray-900 mt-2">{value}</p>
          {subtitle && <p className="text-xs text-gray-400 mt-1">{subtitle}</p>}
        </div>
        <div className={`p-3 rounded-full ${colorClass}`}>
          <Icon className="h-6 w-6 text-white" />
        </div>
      </div>
    </div>
  );

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <h1 className="text-2xl font-bold text-gray-900">Dashboard Overview</h1>
        <button onClick={() => refetch()} className="p-2 text-gray-500 hover:bg-gray-100 rounded-full">
          <RefreshCw className="h-5 w-5" />
        </button>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard 
          title="Total Issues" 
          value={data.totalIssues} 
          icon={FileText} 
          colorClass="bg-blue-500" 
        />
        <StatCard 
          title="Pending Verification" 
          value={data.pendingIssues} 
          icon={AlertCircle} 
          colorClass="bg-yellow-500" 
        />
        <StatCard 
          title="Currently In Progress" 
          value={data.inProgressIssues} 
          icon={Clock} 
          colorClass="bg-indigo-500" 
        />
        <StatCard 
          title="Resolved" 
          value={data.resolvedIssues} 
          icon={CheckCircle} 
          colorClass="bg-green-500" 
          subtitle={`Avg ${data.averageResolutionTimeHours} hrs resolution`}
        />
        
        <StatCard 
          title="Critical Priority" 
          value={data.criticalIssues} 
          icon={ShieldAlert} 
          colorClass="bg-red-500" 
        />
        <StatCard 
          title="Verified (Unassigned)" 
          value={data.verifiedIssues} 
          icon={CheckSquare} 
          colorClass="bg-cyan-500" 
        />
        <StatCard 
          title="Rejected Issues" 
          value={data.rejectedIssues} 
          icon={XCircle} 
          colorClass="bg-gray-500" 
        />
        <StatCard 
          title="New This Week" 
          value={data.issuesLast7Days} 
          icon={ActivityIcon} 
          colorClass="bg-purple-500" 
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Trend Chart */}
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Issue Creation Trend (Last 7 Days)</h3>
          <div className="h-72">
            <ResponsiveContainer width="100%" height="100%">
              <BarChart data={data.dailyIssuesLast7Days} margin={{ top: 20, right: 30, left: 0, bottom: 0 }}>
                <CartesianGrid strokeDasharray="3 3" vertical={false} />
                <XAxis dataKey="date" tickFormatter={(date) => format(new Date(date), 'MMM d')} />
                <YAxis allowDecimals={false} />
                <Tooltip labelFormatter={(label) => format(new Date(label as string), 'MMM d, yyyy')} />
                <Bar dataKey="count" fill="#3B82F6" radius={[4, 4, 0, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        {/* Category Pie */}
        <div className="bg-white rounded-lg shadow p-6">
          <h3 className="text-lg font-semibold text-gray-900 mb-4">Issues by Category</h3>
          <div className="h-72">
            {categoryData.length > 0 ? (
              <ResponsiveContainer width="100%" height="100%">
                <PieChart>
                  <Pie
                    data={categoryData}
                    cx="50%"
                    cy="50%"
                    innerRadius={60}
                    outerRadius={100}
                    paddingAngle={5}
                    dataKey="value"
                    label={({ name, percent }) => `${name} ${((percent || 0) * 100).toFixed(0)}%`}
                  >
                    {categoryData.map((_, index) => (
                      <Cell key={`cell-${index}`} fill={COLORS[index % COLORS.length]} />
                    ))}
                  </Pie>
                  <Tooltip />
                </PieChart>
              </ResponsiveContainer>
            ) : (
              <div className="flex items-center justify-center h-full text-gray-500">No data available</div>
            )}
          </div>
        </div>
      </div>

      <div className="bg-white rounded-lg shadow p-6">
        <div className="flex justify-between items-center mb-4">
          <h3 className="text-lg font-semibold text-gray-900">Recent Issues</h3>
          <Link to="/admin/issues" className="text-sm text-blue-600 hover:underline">View All</Link>
        </div>
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Title</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">Date</th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">Action</th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {data.recentIssues.map((issue: any) => (
                <tr key={issue.id}>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">{issue.title}</td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <span className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-gray-100 text-gray-800">
                      {issue.status}
                    </span>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {format(new Date(issue.createdAt), 'MMM d')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <Link to={`/admin/issues/${issue.id}`} className="text-blue-600 hover:text-blue-900">View</Link>
                  </td>
                </tr>
              ))}
              {data.recentIssues.length === 0 && (
                <tr>
                  <td colSpan={4} className="px-6 py-4 text-center text-sm text-gray-500">No recent issues found.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};

// Simple Icon component for the "New This Week" card to avoid missing import
const ActivityIcon = (props: any) => (
  <svg {...props} xmlns="http://www.w3.org/2000/svg" width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
    <polyline points="22 12 18 12 15 21 9 3 6 12 2 12"></polyline>
  </svg>
);
