import { useEffect, useState } from 'react';
import api from '../../api/axios';
import type { Issue, PagedResult } from '../../types/issue';
import { Link } from 'react-router-dom';

export const IssueList = () => {
  const [issues, setIssues] = useState<Issue[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchIssues = async () => {
      try {
        const { data } = await api.get<PagedResult<Issue>>('/issues');
        setIssues(data.items);
      } catch (error) {
        console.error('Error fetching issues', error);
      } finally {
        setLoading(false);
      }
    };
    fetchIssues();
  }, []);

  if (loading) return <div className="p-8 text-center">Loading issues...</div>;

  return (
    <div className="max-w-6xl mx-auto p-6 mt-8">
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Recent Issues</h1>
        <Link to="/report" className="px-4 py-2 bg-indigo-600 text-white rounded hover:bg-indigo-700">
          Report New Issue
        </Link>
      </div>

      {issues.length === 0 ? (
        <div className="bg-white p-8 shadow rounded-lg text-center text-gray-500">
          No issues reported yet. Be the first to report an issue in your area!
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {issues.map(issue => (
            <div key={issue.id} className="bg-white p-6 shadow rounded-lg flex flex-col">
              <div className="flex justify-between items-start mb-4">
                <h2 className="text-xl font-semibold truncate pr-2">{issue.title}</h2>
                <span className="px-2 py-1 text-xs font-semibold rounded bg-blue-100 text-blue-800">
                  {issue.status}
                </span>
              </div>
              <p className="text-gray-600 text-sm mb-4 line-clamp-2">{issue.description}</p>
              <div className="mt-auto pt-4 border-t border-gray-100">
                <p className="text-xs text-gray-500 mb-1">📍 {issue.address}</p>
                <p className="text-xs text-gray-500">🏷️ {issue.categoryName} • 👤 {issue.userName}</p>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};
