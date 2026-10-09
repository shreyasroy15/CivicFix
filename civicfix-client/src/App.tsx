import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from './contexts/AuthContext';
import { ProtectedRoute } from './components/ProtectedRoute';
import { Login } from './pages/Login';
import { Register } from './pages/Register';
import { Dashboard } from './pages/Dashboard';
import { ReportIssue } from './pages/Issues/ReportIssue';
import { IssueList } from './pages/Issues/IssueList';
import { AdminLayout } from './components/AdminLayout';
import { AdminIssues } from './pages/Admin/AdminIssues';
import { AdminIssueDetails } from './pages/Admin/AdminIssueDetails';
import { AdminDashboard } from './pages/Admin/AdminDashboard';

function App() {
  return (
    <AuthProvider>
      <Router>
        <div className="min-h-screen bg-gray-100">
          {/* We would add a navigation bar here */}
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route path="/register" element={<Register />} />
            <Route path="/issues" element={<IssueList />} />
            <Route element={<ProtectedRoute />}>
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/report" element={<ReportIssue />} />
              <Route path="/" element={<Navigate to="/issues" replace />} />
              
              <Route path="/admin" element={<AdminLayout />}>
                <Route path="dashboard" element={<AdminDashboard />} />
                <Route path="issues" element={<AdminIssues />} />
                <Route path="issues/:id" element={<AdminIssueDetails />} />
                <Route path="departments" element={<div>Departments (Pending)</div>} />
                <Route path="staff" element={<div>Staff (Pending)</div>} />
                <Route path="categories" element={<div>Categories (Pending)</div>} />
                <Route path="analytics" element={<div>Analytics (Pending)</div>} />
                <Route path="audit-logs" element={<div>Audit Logs (Pending)</div>} />
              </Route>
            </Route>
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
}

export default App;
