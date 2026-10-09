import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import * as z from 'zod';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import api from '../api/axios';
import { useState } from 'react';
import { MapPin, Eye, EyeOff, CheckCircle2, Mail, Lock, ShieldCheck, User } from 'lucide-react';
import civicIllustration from '../assets/civic_illustration.jpg';
import { useAuth0 } from '@auth0/auth0-react';

const registerSchema = z.object({
  username: z.string().min(3, 'Username must be at least 3 characters'),
  email: z.string().email('Invalid email address'),
  password: z.string().min(6, 'Password must be at least 6 characters'),
  confirmPassword: z.string()
}).refine((data) => data.password === data.confirmPassword, {
  message: "Passwords don't match",
  path: ["confirmPassword"],
});

type RegisterFormValues = z.infer<typeof registerSchema>;

export const Register = () => {
  const { login } = useAuth();
  const { loginWithRedirect } = useAuth0();
  const navigate = useNavigate();
  const [error, setError] = useState<string | null>(null);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm<RegisterFormValues>({
    resolver: zodResolver(registerSchema)
  });

  const onSubmit = async (data: RegisterFormValues) => {
    try {
      setError(null);
      const response = await api.post('/auth/register', {
        username: data.username,
        email: data.email,
        password: data.password
      });
      login(response.data);
      navigate('/dashboard');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Registration failed. Please try again.');
    }
  };

  return (
    <div className="flex min-h-screen bg-slate-50 font-sans">
      {/* Left Panel - Hidden on mobile */}
      <div className="hidden lg:flex lg:w-1/2 bg-slate-900 flex-col relative overflow-hidden">
        {/* Background decorative elements */}
        <div className="absolute inset-0 z-0">
          <img 
            src={civicIllustration} 
            alt="Smart city illustration" 
            className="w-full h-full object-cover opacity-40 mix-blend-screen object-center"
          />
          {/* Gradient overlay to ensure text readability */}
          <div className="absolute inset-0 bg-gradient-to-b from-slate-900/90 via-slate-900/50 to-transparent" />
        </div>
        
        <div className="relative z-10 p-12 flex flex-col h-full justify-start pt-20">
          <div className="flex items-center gap-3 mb-12">
            <div className="bg-blue-600 p-2.5 rounded-xl shadow-lg shadow-blue-900/50">
              <MapPin className="text-white w-8 h-8" />
            </div>
            <span className="text-3xl font-bold text-white tracking-tight">CivicFix</span>
          </div>
          
          <h1 className="text-4xl md:text-5xl font-extrabold text-white mb-6 leading-tight">
            Make your <span className="text-blue-500">voice heard.</span>
          </h1>
          <p className="text-lg text-slate-300 mb-12 max-w-lg leading-relaxed">
            Join the community platform bridging the gap between citizens and municipal authorities.
          </p>

          <div className="space-y-5">
            {[
              'Report local issues with photos and location',
              'Track resolution progress in real-time',
              'Vote and support community reports'
            ].map((feature, i) => (
              <div key={i} className="flex items-center gap-4">
                <CheckCircle2 className="text-blue-500 w-6 h-6 flex-shrink-0" />
                <span className="text-slate-200 font-medium text-lg">{feature}</span>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* Right Panel - Form */}
      <div className="flex w-full lg:w-1/2 items-center justify-center p-8 sm:p-12 overflow-y-auto">
        <div className="w-full max-w-md space-y-8 bg-white p-10 rounded-2xl shadow-xl shadow-slate-200/50 my-auto">
          {/* Mobile Branding (only visible when left panel is hidden) */}
          <div className="flex lg:hidden flex-col items-center justify-center mb-10">
            <div className="bg-blue-600 p-3 rounded-2xl shadow-lg shadow-blue-600/30 mb-4">
              <MapPin className="text-white w-8 h-8" />
            </div>
            <span className="text-3xl font-extrabold text-slate-900 tracking-tight">CivicFix</span>
          </div>

          <div className="text-center">
            <h2 className="text-3xl font-bold tracking-tight text-slate-900">
              Create an account
            </h2>
            <p className="mt-3 text-sm text-slate-500">
              Join CivicFix and start improving your community today.
            </p>
          </div>

          <form className="mt-10 space-y-5" onSubmit={handleSubmit(onSubmit)}>
            {error && (
              <div className="bg-red-50 border-l-4 border-red-500 p-4 rounded-md shadow-sm">
                <p className="text-sm font-medium text-red-800">{error}</p>
              </div>
            )}
            
            <div>
              <label htmlFor="username" className="block text-sm font-medium text-slate-700">Full Name</label>
              <div className="mt-2 relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <User className="h-5 w-5 text-slate-400" />
                </div>
                <input
                  id="username"
                  type="text"
                  {...register('username')}
                  className="block w-full rounded-xl border border-slate-300 bg-white pl-10 px-4 py-3 text-slate-900 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all sm:text-sm"
                  placeholder="Jane Doe"
                />
              </div>
              {errors.username && <p className="mt-2 text-sm text-red-500">{errors.username.message}</p>}
            </div>

            <div>
              <label htmlFor="email" className="block text-sm font-medium text-slate-700">Email address</label>
              <div className="mt-2 relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Mail className="h-5 w-5 text-slate-400" />
                </div>
                <input
                  id="email"
                  type="email"
                  {...register('email')}
                  className="block w-full rounded-xl border border-slate-300 bg-white pl-10 px-4 py-3 text-slate-900 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all sm:text-sm"
                  placeholder="Email address"
                />
              </div>
              {errors.email && <p className="mt-2 text-sm text-red-500">{errors.email.message}</p>}
            </div>

            <div>
              <label htmlFor="password" className="block text-sm font-medium text-slate-700">Password</label>
              <div className="mt-2 relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-slate-400" />
                </div>
                <input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  {...register('password')}
                  className="block w-full rounded-xl border border-slate-300 bg-white pl-10 px-4 py-3 text-slate-900 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all sm:text-sm pr-12"
                  placeholder="Password"
                />
                <button 
                  type="button" 
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 focus:outline-none p-1 rounded-md"
                >
                  {showPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
              <p className="mt-1.5 text-xs text-slate-500">Must be at least 6 characters</p>
              {errors.password && <p className="mt-2 text-sm text-red-500">{errors.password.message}</p>}
            </div>

            <div>
              <label htmlFor="confirmPassword" className="block text-sm font-medium text-slate-700">Confirm Password</label>
              <div className="mt-2 relative">
                <div className="absolute inset-y-0 left-0 pl-3 flex items-center pointer-events-none">
                  <Lock className="h-5 w-5 text-slate-400" />
                </div>
                <input
                  id="confirmPassword"
                  type={showConfirmPassword ? "text" : "password"}
                  {...register('confirmPassword')}
                  className="block w-full rounded-xl border border-slate-300 bg-white pl-10 px-4 py-3 text-slate-900 shadow-sm focus:border-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500/20 transition-all sm:text-sm pr-12"
                  placeholder="Confirm password"
                />
                <button 
                  type="button" 
                  onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-400 hover:text-slate-600 focus:outline-none p-1 rounded-md"
                >
                  {showConfirmPassword ? <EyeOff className="w-5 h-5" /> : <Eye className="w-5 h-5" />}
                </button>
              </div>
              {errors.confirmPassword && <p className="mt-2 text-sm text-red-500">{errors.confirmPassword.message}</p>}
            </div>

            <div className="pt-4">
              <button
                type="submit"
                disabled={isSubmitting}
                className="flex w-full justify-center items-center rounded-xl bg-blue-600 py-3 px-4 text-sm font-semibold text-white shadow-sm hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2 transition-all disabled:opacity-70 disabled:cursor-not-allowed"
              >
                {isSubmitting ? (
                  <>
                    <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                      <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                      <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                    </svg>
                    Creating account...
                  </>
                ) : 'Create account'}
              </button>
            </div>
            
            <div className="relative my-6">
              <div className="absolute inset-0 flex items-center">
                <div className="w-full border-t border-slate-200"></div>
              </div>
              <div className="relative flex justify-center text-sm">
                <span className="px-2 bg-white text-slate-500">or continue with</span>
              </div>
            </div>

            <div className="grid grid-cols-2 gap-4">
              <button
                type="button"
                onClick={() => loginWithRedirect({ authorizationParams: { connection: 'google-oauth2' } })}
                className="flex w-full justify-center items-center gap-2 rounded-xl border border-slate-300 bg-white py-3 px-4 text-sm font-semibold text-slate-700 shadow-sm hover:bg-slate-50 focus:outline-none focus:ring-2 focus:ring-slate-500 focus:ring-offset-2 transition-all"
              >
                <svg className="w-5 h-5" viewBox="0 0 24 24">
                  <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z" fill="#4285F4"/>
                  <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z" fill="#34A853"/>
                  <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z" fill="#FBBC05"/>
                  <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z" fill="#EA4335"/>
                </svg>
                Google
              </button>
              
              <button
                type="button"
                onClick={() => loginWithRedirect({ authorizationParams: { connection: 'apple' } })}
                className="flex w-full justify-center items-center gap-2 rounded-xl border border-slate-300 bg-black py-3 px-4 text-sm font-semibold text-white shadow-sm hover:bg-slate-900 focus:outline-none focus:ring-2 focus:ring-slate-500 focus:ring-offset-2 transition-all"
              >
                <svg className="w-5 h-5 fill-current" viewBox="0 0 24 24">
                  <path d="M17.05 20.28c-.98.95-2.05.8-3.08.35-1.09-.46-2.09-.48-3.24 0-1.44.62-2.2.44-3.06-.35C2.79 15.25 3.51 7.59 9.05 7.31c1.35.07 2.29.74 3.08.8 1.18-.24 2.31-.93 3.57-.84 1.51.15 2.95.97 3.67 2.06-3.15 1.83-2.62 5.92.51 7.21-.76 1.45-1.63 2.75-2.83 3.74M12.03 7.25c-.15-2.23 1.66-4.07 3.74-4.25.29 2.58-2.34 4.5-3.74 4.25"/>
                </svg>
                Apple
              </button>
            </div>
            
            <div className="text-center text-sm text-slate-600 pt-6">
              Already have an account?{' '}
              <Link to="/login" className="font-semibold text-blue-600 hover:text-blue-700 transition-colors">
                Sign in
              </Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
};
