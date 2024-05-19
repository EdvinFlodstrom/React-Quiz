import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'https://localhost:7030/api',
  headers: {
    'Content-Type': 'application/json',
  },
});

export default apiClient;
