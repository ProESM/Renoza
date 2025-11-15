export const environment = {
  production: true,
  // В Docker можно переопределить через window.env или использовать переменные окружения
  apiUrl: (window as any).env?.apiUrl || 'http://localhost:8080/api'
};
