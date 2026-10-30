import { STRINGS } from '../constants/strings';

export function formatProgress(
  completed: number,
  total: number,
  percentage: number
): string {
  if (total <= 0) {
    return STRINGS.progress.summaryPattern
      .replace('{completed}', '0')
      .replace('{total}', '0')
      .replace('{percentage}', '0');
  }

  const safePercentage = Math.min(100, Math.max(0, percentage));

  return STRINGS.progress.summaryPattern
    .replace('{completed}', String(completed))
    .replace('{total}', String(total))
    .replace('{percentage}', String(safePercentage));
}

export function clampPercentage(percentage: number): number {
  if (Number.isNaN(percentage) || percentage < 0) {
    return 0;
  }
  if (percentage > 100) {
    return 100;
  }
  return percentage;
}
