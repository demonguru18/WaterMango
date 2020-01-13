export interface Plant {
  id: number;
  name: string;
  imageUrl: string;
  description: string;
  lastWateredAt: string;
  status: boolean;
  totalWaterTime: number;
  jobId: string;
  nextWateredAt: string;
}
