import dayjs from 'dayjs';
import 'dayjs/locale/es';

dayjs.locale('es');

export function formatDate(date: string | Date): string {
  return dayjs(date).format('DD/MM/YYYY');
}

export function formatDateTime(date: string | Date): string {
  return dayjs(date).format('DD/MM/YYYY HH:mm');
}

export function formatCurrency(amount: number): string {
  return new Intl.NumberFormat('es-CO', {
    style: 'currency',
    currency: 'COP',
    minimumFractionDigits: 0,
    maximumFractionDigits: 0,
  }).format(amount);
}

export function formatDateForInput(date: string | Date): string {
  return dayjs(date).format('YYYY-MM-DD');
}
