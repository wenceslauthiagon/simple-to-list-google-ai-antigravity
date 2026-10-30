import { describe, it, expect } from 'vitest';
import { formatProgress, clampPercentage } from './formatters';

describe('formatters', () => {
  it('TC-UTIL-0001 - formats progress string matching standard pattern', () => {
    const result = formatProgress(3, 8, 38);
    expect(result).toBe('3 of 8 completed, 38%');
  });

  it('TC-UTIL-0002 - formats empty progress safely without division or NaN errors', () => {
    const result = formatProgress(0, 0, 0);
    expect(result).toBe('0 of 0 completed, 0%');
  });

  it('TC-UTIL-0003 - clamps percentages strictly between 0 and 100', () => {
    expect(clampPercentage(-10)).toBe(0);
    expect(clampPercentage(150)).toBe(100);
    expect(clampPercentage(42)).toBe(42);
  });
});
