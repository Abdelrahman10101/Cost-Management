# Cost-Management
Here's a comprehensive `README.md` file for your Cost Management API project:


# Cost Management API

A comprehensive API for managing business costs, invoices, and tax calculations built with ASP.NET Core.

## Features

- **Cost Entry Management**: Track business expenses with categories, amounts, and descriptions
- **Invoice Generation**: Create professional invoices with items, taxes, and discounts
- **Invoice Reminders**: Automated notifications for upcoming and overdue invoices
- **Tax Calculations**: Automatic tax computation based on regional rates
- **In-Memory Database**: No setup required for development (data persists until app restarts)

## API Endpoints

### Cost Entries
- `GET /api/costentries` - List all cost entries
- `POST /api/costentries` - Create new cost entry
- `GET /api/costentries/{id}` - Get specific cost entry

### Invoices
- `GET /api/invoices` - List all invoices
- `POST /api/invoices` - Create new invoice
- `GET /api/invoices/{id}` - Get specific invoice
- `PUT /api/invoices/{id}` - Update existing invoice
- `POST /api/invoices/{id}/reminders` - Send invoice reminder

### Tax Calculations
- `GET /api/tax/calculate?subtotal=X&region=Y` - Calculate tax amount
- `GET /api/tax/rates/{region}` - Get tax rate for specific region

## Getting Started

### Prerequisites
- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- Postman or similar API testing tool

### Installation
1. Clone the repository:
   ```bash
   git clone https://github.com/yourusername/cost-management-api.git
   ```
2. Open the solution in Visual Studio
3. Build the solution (Ctrl+Shift+B)
4. Run the project (F5)

### Testing the API
The API will be available at `https://localhost:5047` (or your configured port). Use Postman or Swagger UI to test endpoints.

## Sample Requests

### Create Cost Entry
```http
POST /api/costentries
Content-Type: application/json

{
  "category": "Office Supplies",
  "amount": 99.99,
  "date": "2023-05-20",
  "description": "Printer ink cartridges"
}
```

### Generate Invoice
```http
POST /api/invoices
Content-Type: application/json

{
  "clientId": "CL001",
  "items": [
    {
      "name": "Web Development",
      "quantity": 1,
      "unitPrice": 2000.00
    }
  ],
  "discounts": 0.10
}
```

## Data Structure

### Cost Entry
```json
{
  "id": 1,
  "category": "Software",
  "amount": 499.99,
  "date": "2023-05-18",
  "description": "Project management tool",
  "createdAt": "2023-05-18T10:30:00"
}
```

### Invoice
```json
{
  "id": 1001,
  "clientId": "CL001",
  "items": [
    {
      "name": "Consulting",
      "quantity": 5,
      "unitPrice": 200.00
    }
  ],
  "subtotal": 1000.00,
  "discounts": 0.10,
  "taxRate": 0.08,
  "taxAmount": 72.00,
  "total": 972.00,
  "dueDate": "2023-06-20",
  "status": "pending"
}
```

## Development Notes

- **In-Memory Storage**: All data is stored in memory and will reset when the application restarts
- **Error Handling**: Basic validation and error responses implemented
- **Testing**: Test endpoints using Postman collection included in `/docs`

## Future Enhancements

1. Add persistent database (SQL Server/PostgreSQL)
2. Implement authentication/authorization
3. Add reporting endpoints
4. Support PDF invoice generation
5. Add bulk operations


---

For any questions or support, please contact [Abdelrahman_Hesham@outlook.com].
```
