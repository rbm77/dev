// Column sizes from Database/Schemas/*.sql. The IN parameters of every insert_*,
// update_*, authenticate_user and reset_password procedure use the same sizes,
// so these are the real limits the API enforces. Single source for zod + inputs.
export const LIMITS = {
  user: { username: 50, password: 10 },
  personalData: { identityDocument: 12, name: 50, lastName: 50, address: 130, phoneNumber: 12, email: 50 },
  company: { name: 50, phoneNumber: 12, email: 50 },
  role: { description: 30 },
  grade: { description: 30 },
  route: { name: 30, description: 70 },
  vehicle: { licensePlate: 10, make: 15, model: 15 },
  student: { name: 50, lastName: 50, address: 130, identityDocument: 12 },
  contact: { phoneNumber: 12, description: 50 },
  driver: { licenseNumber: 17 },
  maintenance: { description: 100 },
  incident: { description: 150, correctiveActions: 150 },
  customTransport: { description: 150 },
  expense: { description: 130 },
  payment: { receiptReference: 50 },
  vacation: { description: 50 },
  specificExemption: { description: 50 },
  periodicExemption: { description: 50 },
  emailAccount: { emailAddress: 50, appPassword: 30, imapHost: 50 },
  emailSender: { senderAddress: 50, description: 50 },
  whatsappAccount: { phoneNumberId: 32, accessToken: 512 },
} as const;

export const NUMBER_LIMITS = {
  /** DECIMAL(10,2) */
  money: { max: 99_999_999.99 },
  /** MySQL INT */
  int: { max: 2_147_483_647 },
  percentage: { min: 0.01, max: 100 },
  manufactureYear: { min: 1950 },
  vehicleCapacity: { min: 1, max: 999 },
  imapPort: { min: 1, max: 65_535 },
  daysToNextPayment: { min: 1, max: 366 },
  /** QrCodeService: at most 20 items, size 1–50. */
  qrCodes: { maxItems: 20, maxSize: 50 },
} as const;
