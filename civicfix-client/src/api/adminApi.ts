import api from './axios';

export const adminApi = {
  // Issues
  getIssues: async (params: any) => {
    const { data } = await api.get('/admin/issues', { params });
    return data;
  },
  getIssueById: async (id: string) => {
    const { data } = await api.get(`/admin/issues/${id}`);
    return data;
  },
  verifyIssue: async (id: string) => {
    await api.put(`/admin/issues/${id}/verify`);
  },
  rejectIssue: async (id: string, reason: string) => {
    await api.put(`/admin/issues/${id}/reject`, { reason });
  },
  updatePriority: async (id: string, priority: string) => {
    await api.put(`/admin/issues/${id}/priority`, { priority });
  },
  updateStatus: async (id: string, status: string) => {
    await api.put(`/admin/issues/${id}/status`, { status });
  },
  assignIssue: async (id: string, departmentId: number, assignedStaffId?: string) => {
    await api.put(`/admin/issues/${id}/assign`, { departmentId, assignedStaffId });
  },
  addNote: async (id: string, note: string) => {
    await api.post(`/admin/issues/${id}/notes`, { note });
  },
  getIssueHistory: async (id: string) => {
    const { data } = await api.get(`/admin/issues/${id}/history`);
    return data;
  },

  // Departments
  getDepartments: async () => {
    const { data } = await api.get('/departments'); // Or /admin/departments if we move it
    return data;
  },
  getStaffByDepartment: async (deptId: number) => {
    const { data } = await api.get(`/departments/${deptId}/staff`);
    return data;
  },

  // Staff
  getStaff: async (params: any) => {
    const { data } = await api.get('/admin/staff', { params });
    return data;
  }
};
