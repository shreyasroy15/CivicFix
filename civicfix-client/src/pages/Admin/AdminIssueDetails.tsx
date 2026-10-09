import { useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { adminApi } from '../../api/adminApi';
import { ArrowLeft, CheckCircle, XCircle, AlertTriangle, MessageSquare, Clock } from 'lucide-react';
import { format } from 'date-fns';

export const AdminIssueDetails = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const queryClient = useQueryClient();
  const [rejectReason, setRejectReason] = useState('');
  const [showRejectModal, setShowRejectModal] = useState(false);
  const [note, setNote] = useState('');
  const [isUpdatingStatus, setIsUpdatingStatus] = useState(false);

  const { data: issue, isLoading, isError } = useQuery({
    queryKey: ['admin-issue', id],
    queryFn: () => adminApi.getIssueById(id!),
    enabled: !!id,
  });

  const { data: history } = useQuery({
    queryKey: ['admin-issue-history', id],
    queryFn: () => adminApi.getIssueHistory(id!),
    enabled: !!id,
  });

  const verifyMutation = useMutation({
    mutationFn: () => adminApi.verifyIssue(id!),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-issue', id] });
      queryClient.invalidateQueries({ queryKey: ['admin-issue-history', id] });
    },
  });

  const rejectMutation = useMutation({
    mutationFn: (reason: string) => adminApi.rejectIssue(id!, reason),
    onSuccess: () => {
      setShowRejectModal(false);
      setRejectReason('');
      queryClient.invalidateQueries({ queryKey: ['admin-issue', id] });
      queryClient.invalidateQueries({ queryKey: ['admin-issue-history', id] });
    },
  });

  const priorityMutation = useMutation({
    mutationFn: (priority: string) => adminApi.updatePriority(id!, priority),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['admin-issue', id] });
      queryClient.invalidateQueries({ queryKey: ['admin-issue-history', id] });
    },
  });

  const statusMutation = useMutation({
    mutationFn: (status: string) => adminApi.updateStatus(id!, status),
    onSuccess: () => {
      setIsUpdatingStatus(false);
      queryClient.invalidateQueries({ queryKey: ['admin-issue', id] });
      queryClient.invalidateQueries({ queryKey: ['admin-issue-history', id] });
    },
  });

  const noteMutation = useMutation({
    mutationFn: (newNote: string) => adminApi.addNote(id!, newNote),
    onSuccess: () => {
      setNote('');
      queryClient.invalidateQueries({ queryKey: ['admin-issue', id] });
      queryClient.invalidateQueries({ queryKey: ['admin-issue-history', id] });
    },
  });

  if (isLoading) {
    return (
      <div className="flex justify-center py-12">
        <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600"></div>
      </div>
    );
  }

  if (isError || !issue) {
    return (
      <div className="text-center py-12">
        <h2 className="text-xl font-semibold text-gray-900">Issue not found</h2>
        <button onClick={() => navigate('/admin/issues')} className="mt-4 text-blue-600 hover:underline">
          Return to issues list
        </button>
      </div>
    );
  }

  const handleReject = () => {
    if (!rejectReason.trim()) return;
    rejectMutation.mutate(rejectReason);
  };

  return (
    <div className="space-y-6">
      <div className="flex items-center gap-4">
        <Link to="/admin/issues" className="p-2 hover:bg-gray-100 rounded-full transition-colors">
          <ArrowLeft className="h-5 w-5 text-gray-600" />
        </Link>
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Issue Details</h1>
          <p className="text-sm text-gray-500">ID: {issue.id}</p>
        </div>
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          {/* Main Info */}
          <div className="bg-white rounded-lg shadow p-6">
            <div className="flex justify-between items-start mb-4">
              <h2 className="text-xl font-semibold text-gray-900">{issue.title}</h2>
              <div className="flex gap-2">
                <span className={`px-3 py-1 rounded-full text-xs font-semibold
                  ${issue.status === 'PENDING' ? 'bg-yellow-100 text-yellow-800' : 
                    issue.status === 'VERIFIED' ? 'bg-blue-100 text-blue-800' :
                    issue.status === 'RESOLVED' ? 'bg-green-100 text-green-800' :
                    issue.status === 'REJECTED' ? 'bg-red-100 text-red-800' :
                    'bg-gray-100 text-gray-800'}`}>
                  {issue.status}
                </span>
                <select
                  value={issue.priority}
                  onChange={(e) => priorityMutation.mutate(e.target.value)}
                  disabled={priorityMutation.isPending}
                  className={`px-3 py-1 rounded-full text-xs font-semibold outline-none border-none cursor-pointer
                    ${issue.priority === 'CRITICAL' ? 'bg-red-100 text-red-800' :
                      issue.priority === 'HIGH' ? 'bg-orange-100 text-orange-800' :
                      'bg-gray-100 text-gray-800'}`}
                >
                  <option value="LOW">Low Priority</option>
                  <option value="MEDIUM">Medium Priority</option>
                  <option value="HIGH">High Priority</option>
                  <option value="CRITICAL">Critical Priority</option>
                </select>
              </div>
            </div>
            
            <p className="text-gray-700 whitespace-pre-wrap">{issue.description}</p>
            
            <div className="mt-6 grid grid-cols-2 gap-4 text-sm">
              <div>
                <span className="text-gray-500 block">Category</span>
                <span className="font-medium text-gray-900">{issue.categoryName}</span>
              </div>
              <div>
                <span className="text-gray-500 block">Reported By</span>
                <span className="font-medium text-gray-900">{issue.userName}</span>
              </div>
              <div>
                <span className="text-gray-500 block">Reported Date</span>
                <span className="font-medium text-gray-900">{format(new Date(issue.createdAt), 'MMM d, yyyy h:mm a')}</span>
              </div>
              <div>
                <span className="text-gray-500 block">Location</span>
                <span className="font-medium text-gray-900">{issue.address || 'No address provided'}</span>
              </div>
              <div>
                <span className="text-gray-500 block">Department</span>
                <span className="font-medium text-gray-900">{issue.departmentName || 'Unassigned'}</span>
              </div>
              <div>
                <span className="text-gray-500 block">Assigned Staff</span>
                <span className="font-medium text-gray-900">{issue.assignedStaffName || 'Unassigned'}</span>
              </div>
            </div>
          </div>

          {/* Workflow Actions */}
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4">Workflow Actions</h3>
            <div className="flex flex-wrap gap-4">
              {issue.status === 'PENDING' && (
                <button
                  onClick={() => verifyMutation.mutate()}
                  disabled={verifyMutation.isPending}
                  className="flex items-center px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:opacity-50"
                >
                  <CheckCircle className="mr-2 h-4 w-4" />
                  Verify Issue
                </button>
              )}
              
              {(issue.status === 'PENDING' || issue.status === 'VERIFIED') && (
                <button
                  onClick={() => setShowRejectModal(true)}
                  className="flex items-center px-4 py-2 bg-red-600 text-white rounded hover:bg-red-700"
                >
                  <XCircle className="mr-2 h-4 w-4" />
                  Reject Issue
                </button>
              )}

              {/* Status Override */}
              <div className="relative">
                {isUpdatingStatus ? (
                  <div className="flex gap-2">
                    <select
                      className="border border-gray-300 rounded px-3 py-2 text-sm focus:ring-blue-500 focus:border-blue-500"
                      onChange={(e) => {
                        if (confirm(`Are you sure you want to override status to ${e.target.value}?`)) {
                          statusMutation.mutate(e.target.value);
                        } else {
                          setIsUpdatingStatus(false);
                        }
                      }}
                      defaultValue={issue.status}
                    >
                      <option value="PENDING">PENDING</option>
                      <option value="VERIFIED">VERIFIED</option>
                      <option value="ASSIGNED">ASSIGNED</option>
                      <option value="IN_PROGRESS">IN_PROGRESS</option>
                      <option value="RESOLVED">RESOLVED</option>
                      <option value="CLOSED">CLOSED</option>
                    </select>
                    <button 
                      onClick={() => setIsUpdatingStatus(false)}
                      className="px-3 py-2 text-sm text-gray-600 hover:text-gray-900"
                    >
                      Cancel
                    </button>
                  </div>
                ) : (
                  <button
                    onClick={() => setIsUpdatingStatus(true)}
                    className="flex items-center px-4 py-2 border border-gray-300 text-gray-700 rounded hover:bg-gray-50"
                  >
                    <AlertTriangle className="mr-2 h-4 w-4 text-orange-500" />
                    Override Status
                  </button>
                )}
              </div>
            </div>
          </div>

          {/* Images */}
          {issue.imageUrls && issue.imageUrls.length > 0 && (
            <div className="bg-white rounded-lg shadow p-6">
              <h3 className="text-lg font-semibold text-gray-900 mb-4">Attached Images</h3>
              <div className="grid grid-cols-2 md:grid-cols-3 gap-4">
                {issue.imageUrls.map((url: string, index: number) => (
                  <a key={index} href={url} target="_blank" rel="noopener noreferrer" className="block relative aspect-square bg-gray-100 rounded overflow-hidden group">
                    <img src={url} alt={`Issue ${index + 1}`} className="object-cover w-full h-full group-hover:scale-105 transition-transform" />
                  </a>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Sidebar */}
        <div className="space-y-6">
          {/* Internal Notes */}
          <div className="bg-yellow-50 rounded-lg shadow p-6 border border-yellow-200">
            <h3 className="text-lg font-semibold text-yellow-900 mb-4 flex items-center">
              <MessageSquare className="mr-2 h-5 w-5" />
              Internal Note
            </h3>
            
            {issue.adminNote ? (
              <div className="mb-4 p-4 bg-white rounded border border-yellow-300 text-sm text-gray-800 whitespace-pre-wrap">
                {issue.adminNote}
              </div>
            ) : (
              <p className="text-sm text-yellow-700 mb-4">No internal notes added yet.</p>
            )}

            <div className="space-y-3">
              <textarea
                value={note}
                onChange={(e) => setNote(e.target.value)}
                placeholder="Add or update internal note (Not visible to public)"
                className="w-full px-3 py-2 border border-yellow-300 rounded focus:ring-yellow-500 focus:border-yellow-500 text-sm min-h-[100px] outline-none"
              />
              <button
                onClick={() => {
                  if (note.trim()) noteMutation.mutate(note);
                }}
                disabled={noteMutation.isPending || !note.trim()}
                className="w-full px-4 py-2 bg-yellow-600 text-white rounded text-sm font-medium hover:bg-yellow-700 disabled:opacity-50"
              >
                {noteMutation.isPending ? 'Saving...' : 'Save Note'}
              </button>
            </div>
          </div>

          {/* Audit History */}
          <div className="bg-white rounded-lg shadow p-6">
            <h3 className="text-lg font-semibold text-gray-900 mb-4 flex items-center">
              <Clock className="mr-2 h-5 w-5" />
              Audit History
            </h3>
            
            <div className="space-y-4">
              {history?.map((log: any) => (
                <div key={log.id} className="text-sm border-l-2 border-gray-200 pl-4 py-1">
                  <div className="font-medium text-gray-900">{log.action}</div>
                  <div className="text-xs text-gray-500">
                    {format(new Date(log.timestamp), 'MMM d, yyyy h:mm a')}
                  </div>
                </div>
              ))}
              {!history?.length && <p className="text-sm text-gray-500">No history found.</p>}
            </div>
          </div>
        </div>
      </div>

      {/* Reject Modal */}
      {showRejectModal && (
        <div className="fixed inset-0 z-50 overflow-y-auto">
          <div className="flex items-center justify-center min-h-screen px-4 pt-4 pb-20 text-center sm:p-0">
            <div className="fixed inset-0 transition-opacity bg-gray-500 bg-opacity-75" onClick={() => setShowRejectModal(false)} />

            <div className="relative inline-block w-full max-w-lg p-6 overflow-hidden text-left align-middle transition-all transform bg-white shadow-xl rounded-lg">
              <h3 className="text-lg font-medium leading-6 text-gray-900 mb-4">
                Reject Issue {issue.id.substring(0, 8)}
              </h3>
              
              <div className="mb-4">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Rejection Reason (Required)
                </label>
                <textarea
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded focus:ring-blue-500 focus:border-blue-500 outline-none min-h-[100px]"
                  placeholder="Explain why this issue is being rejected..."
                />
              </div>

              {rejectMutation.isError && (
                <div className="mb-4 p-3 bg-red-50 text-red-700 rounded text-sm">
                  Failed to reject issue. Make sure you provided a reason.
                </div>
              )}

              <div className="flex justify-end gap-3 mt-6">
                <button
                  type="button"
                  onClick={() => setShowRejectModal(false)}
                  className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded hover:bg-gray-50"
                >
                  Cancel
                </button>
                <button
                  type="button"
                  onClick={handleReject}
                  disabled={!rejectReason.trim() || rejectMutation.isPending}
                  className="px-4 py-2 text-sm font-medium text-white bg-red-600 border border-transparent rounded hover:bg-red-700 disabled:opacity-50"
                >
                  {rejectMutation.isPending ? 'Rejecting...' : 'Confirm Rejection'}
                </button>
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
