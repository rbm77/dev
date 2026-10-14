/** GET /students/{id}/contacts */
export type Contact = { id: number; phoneNumber?: string; description?: string; isActive: boolean };

/** POST /students/qr-codes response item (PNG in base64; no student id is returned). */
export type QrCode = { description?: string; qrCodeBase64: string };
