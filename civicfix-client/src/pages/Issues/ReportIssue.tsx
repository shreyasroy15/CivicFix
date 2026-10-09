import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useNavigate } from 'react-router-dom';
import api from '../../api/axios';
import { useState, useEffect } from 'react';
import type { Category } from '../../types/issue';

const issueSchema = z.object({
  title: z.string().min(3, 'Title is required'),
  description: z.string().min(10, 'Please provide more details'),
  categoryId: z.string().min(1, 'Category is required'),
  address: z.string().min(5, 'Address is required'),
  latitude: z.number().optional(),
  longitude: z.number().optional(),
});

type IssueFormValues = z.infer<typeof issueSchema>;

export const ReportIssue = () => {
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [categories, setCategories] = useState<Category[]>([]);
  const [, setImageFiles] = useState<File[]>([]);

  useEffect(() => {
    // We would fetch categories from API, for now mock it if no API available
    // For production, create a CategoryController
    setCategories([
      { id: 1, name: 'Roads & Streets' },
      { id: 2, name: 'Garbage & Cleaning' },
      { id: 3, name: 'Water & Plumbing' },
      { id: 4, name: 'Electricity' }
    ]);
  }, []);

  const { register, handleSubmit, formState: { errors, isSubmitting }, setValue } = useForm<IssueFormValues>({
    resolver: zodResolver(issueSchema)
  });

  const getLocation = () => {
    if (navigator.geolocation) {
      navigator.geolocation.getCurrentPosition(
        (position) => {
          setValue('latitude', position.coords.latitude);
          setValue('longitude', position.coords.longitude);
        },
        () => {
          setError('Unable to retrieve your location');
        }
      );
    } else {
      setError('Geolocation is not supported by your browser');
    }
  };

  const onSubmit = async (data: IssueFormValues) => {
    try {
      setError(null);
      
      // We would upload images first, but skip for this implementation phase
      const imageUrls: string[] = [];

      const payload = {
        title: data.title,
        description: data.description,
        categoryId: parseInt(data.categoryId),
        address: data.address,
        latitude: data.latitude || 0,
        longitude: data.longitude || 0,
        imageUrls
      };

      await api.post('/issues', payload);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to report issue.');
    }
  };

  return (
    <div className="max-w-3xl mx-auto p-6 bg-white shadow rounded-lg mt-8">
      <h1 className="text-2xl font-bold mb-6">Report a Civic Issue</h1>
      
      {error && <div className="bg-red-50 text-red-600 p-4 mb-6 rounded">{error}</div>}

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div>
          <label className="block text-sm font-medium text-gray-700">Title</label>
          <input
            {...register('title')}
            className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          />
          {errors.title && <p className="text-red-500 text-sm mt-1">{errors.title.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Category</label>
          <select
            {...register('categoryId')}
            className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          >
            <option value="">Select a category...</option>
            {categories.map(c => (
              <option key={c.id} value={c.id}>{c.name}</option>
            ))}
          </select>
          {errors.categoryId && <p className="text-red-500 text-sm mt-1">{errors.categoryId.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Description</label>
          <textarea
            {...register('description')}
            rows={4}
            className="mt-1 block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
          />
          {errors.description && <p className="text-red-500 text-sm mt-1">{errors.description.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Address / Location</label>
          <div className="flex gap-2 mt-1">
            <input
              {...register('address')}
              className="block w-full rounded-md border border-gray-300 px-3 py-2 shadow-sm focus:border-indigo-500 focus:ring-indigo-500"
            />
            <button
              type="button"
              onClick={getLocation}
              className="px-4 py-2 bg-gray-200 text-gray-700 rounded hover:bg-gray-300"
            >
              Get GPS
            </button>
          </div>
          {errors.address && <p className="text-red-500 text-sm mt-1">{errors.address.message}</p>}
        </div>

        <div>
          <label className="block text-sm font-medium text-gray-700">Images</label>
          <input
            type="file"
            multiple
            accept="image/*"
            onChange={(e) => {
              if (e.target.files) setImageFiles(Array.from(e.target.files));
            }}
            className="mt-1 block w-full"
          />
          <p className="text-sm text-gray-500 mt-1">Image upload to Cloudinary will be implemented in the next phase.</p>
        </div>

        <button
          type="submit"
          disabled={isSubmitting}
          className="w-full flex justify-center py-2 px-4 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 disabled:bg-indigo-400"
        >
          {isSubmitting ? 'Submitting...' : 'Report Issue'}
        </button>
      </form>
    </div>
  );
};
