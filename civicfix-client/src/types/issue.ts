export interface Category {
  id: number;
  name: string;
}

export interface Issue {
  id: string;
  title: string;
  description: string;
  status: string;
  priority: string;
  latitude: number;
  longitude: number;
  address: string;
  createdAt: string;
  updatedAt?: string;
  userId: string;
  userName: string;
  categoryId: number;
  categoryName: string;
  imageUrls: string[];
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}
