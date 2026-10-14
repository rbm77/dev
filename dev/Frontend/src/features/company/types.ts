/** GET /companies/{id} (Features/Companies/Company.cs) */
export type Company = {
  id: number;
  name?: string;
  phoneNumber?: string;
  email?: string;
  isActive: boolean;
  autoApprovalEnabled: boolean;
};
