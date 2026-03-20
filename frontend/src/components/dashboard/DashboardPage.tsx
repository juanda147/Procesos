import { Box, CircularProgress } from '@mui/material';
import { BarChart, Bar, XAxis, YAxis, Tooltip, ResponsiveContainer, PieChart, Pie, Cell, Legend, type PieLabelRenderProps } from 'recharts';
import { useNavigate } from 'react-router-dom';
import { useDashboard } from '../../hooks/useDashboard';
import { formatDate } from '../../utils/formatters';
import type { Recordatorio } from '../../types/recordatorio';

const COLORS = ['#4a6741', '#8b7355', '#b8860b', '#c0625a', '#7c6992', '#5a8a8a', '#b07c9e', '#6b7d8a'];

function StatCard({ title, value, icon, gradient }: { title: string; value: number; icon: string; gradient: string }) {
  return (
    <div className={`rounded-xl p-5 text-white shadow-md ${gradient}`}>
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium opacity-90">{title}</p>
          <p className="text-3xl font-bold mt-1">{value}</p>
        </div>
        <span className="text-4xl opacity-80">{icon}</span>
      </div>
    </div>
  );
}

function RecordatorioItem({ r }: { r: Recordatorio }) {
  const navigate = useNavigate();
  const isOverdue = new Date(r.fechaVencimiento) < new Date();
  return (
    <div
      className="flex items-center justify-between py-3 px-3 rounded-lg cursor-pointer hover:bg-[#ede8e0] transition-colors"
      onClick={() => navigate(`/procesos/${r.procesoId}`)}
    >
      <div className="min-w-0 flex-1">
        <p className="text-sm font-medium text-[#2d3436] truncate">{r.titulo}</p>
        <p className="text-xs text-[#636e72] truncate">{r.demandanteDelProceso}</p>
      </div>
      <span className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ml-3 ${
        isOverdue
          ? 'bg-red-50 text-red-700 ring-1 ring-red-600/20'
          : 'bg-amber-50 text-amber-700 ring-1 ring-amber-600/20'
      }`}>
        {formatDate(r.fechaVencimiento)}
      </span>
    </div>
  );
}

export default function DashboardPage() {
  const { data, isLoading } = useDashboard();

  if (isLoading) return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box>;
  if (!data) return null;

  return (
    <div>
      <h1 className="text-2xl font-bold text-[#2d3436] mb-6">Panel de Control</h1>

      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        <StatCard title="Total Procesos" value={data.totalProcesos} icon="⚖️" gradient="bg-gradient-to-br from-[#4a6741] to-[#3d5636]" />
        <StatCard title="Procesos Activos" value={data.procesosActivos} icon="✅" gradient="bg-gradient-to-br from-[#5a8a5a] to-[#4a7a4a]" />
        <StatCard title="Recordatorios Pendientes" value={data.recordatoriosPendientes} icon="⏰" gradient="bg-gradient-to-br from-[#b8860b] to-[#9a7209]" />
        <StatCard title="Recordatorios Vencidos" value={data.recordatoriosVencidos} icon="⚠️" gradient="bg-gradient-to-br from-[#c0625a] to-[#a8524a]" />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-12 gap-6">
        <div className="lg:col-span-7">
          <div className="bg-[#faf7f2] rounded-xl shadow-sm border border-[#d5cec4] p-5">
            <h2 className="text-lg font-semibold text-[#2d3436] mb-4">Procesos por Tipo</h2>
            <ResponsiveContainer width="100%" height={Math.max(300, data.procesosPorTipo.length * 40)}>
              <BarChart data={data.procesosPorTipo} layout="vertical" margin={{ left: 10, right: 20 }}>
                <XAxis type="number" allowDecimals={false} />
                <YAxis
                  type="category"
                  dataKey="nombre"
                  width={200}
                  tick={({ x, y, payload }) => {
                    const label = payload.value.length > 28 ? payload.value.slice(0, 28) + '...' : payload.value;
                    return (
                      <text x={x} y={y} dy={4} textAnchor="end" fontSize={11} fill="#2d3436">
                        <title>{payload.value}</title>
                        {label}
                      </text>
                    );
                  }}
                />
                <Tooltip />
                <Bar dataKey="cantidad" fill="#4a6741" radius={[0, 6, 6, 0]} />
              </BarChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="lg:col-span-5">
          <div className="bg-[#faf7f2] rounded-xl shadow-sm border border-[#d5cec4] p-5">
            <h2 className="text-lg font-semibold text-[#2d3436] mb-4">Procesos por Ciudad</h2>
            <ResponsiveContainer width="100%" height={350}>
              <PieChart>
                <Pie
                  data={data.procesosPorCiudad}
                  dataKey="cantidad"
                  nameKey="nombre"
                  cx="50%"
                  cy="45%"
                  innerRadius={50}
                  outerRadius={100}
                  paddingAngle={2}
                  label={(props: PieLabelRenderProps) => {
                    const n = String(props.name || '');
                    const p = Number(props.percent || 0);
                    return `${n.length > 12 ? n.slice(0, 12) + '…' : n} (${(p * 100).toFixed(0)}%)`;
                  }}
                  labelLine={{ strokeWidth: 1 }}
                >
                  {data.procesosPorCiudad.map((_entry, index) => (
                    <Cell key={index} fill={COLORS[index % COLORS.length]} />
                  ))}
                </Pie>
                <Tooltip />
                <Legend
                  verticalAlign="bottom"
                  iconType="circle"
                  iconSize={8}
                  wrapperStyle={{ fontSize: 12, paddingTop: 10 }}
                />
              </PieChart>
            </ResponsiveContainer>
          </div>
        </div>

        <div className="lg:col-span-12">
          <div className="bg-[#faf7f2] rounded-xl shadow-sm border border-[#d5cec4] p-5">
            <h2 className="text-lg font-semibold text-[#2d3436] mb-4">Proximos Recordatorios</h2>
            {data.proximosRecordatorios.length === 0 ? (
              <p className="text-[#636e72] text-sm">No hay recordatorios pendientes</p>
            ) : (
              <div className="divide-y divide-[#e8e2d9]">
                {data.proximosRecordatorios.map((r) => <RecordatorioItem key={r.id} r={r} />)}
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
