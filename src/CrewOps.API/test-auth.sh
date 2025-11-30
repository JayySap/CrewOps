#!/bin/bash

# CrewOps JWT Authentication Test Script
# Run with: chmod +x test-auth.sh && ./test-auth.sh

BASE_URL="http://localhost:5152"

echo "=========================================="
echo "CrewOps JWT Authentication Test Suite"
echo "=========================================="
echo ""

# --- SETUP: Set password for John Smith ---

echo "1. POST - Set password for John Smith (ID 1)"
curl -s -X POST "$BASE_URL/api/auth/set-password" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "password": "password123"
  }' | jq .
echo ""

# --- LOGIN TESTS ---

echo "2. POST - Login with wrong password (should fail)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.smith@example.com",
    "password": "wrongpassword"
  }'
echo ""

echo "3. POST - Login with correct credentials"
LOGIN_RESPONSE=$(curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.smith@example.com",
    "password": "password123"
  }')
echo "$LOGIN_RESPONSE" | jq .
echo ""

# Extract token
TOKEN=$(echo "$LOGIN_RESPONSE" | jq -r '.token')
echo "Extracted Token (first 50 chars): ${TOKEN:0:50}..."
echo ""

# --- PROTECTED ENDPOINT TESTS ---

echo "4. GET - Try to access /api/time/active WITHOUT token (should fail 401)"
curl -s -w "\nHTTP Status: %{http_code}\n" "$BASE_URL/api/time/active"
echo ""

echo "5. GET - Access /api/time/active WITH token (should work)"
curl -s -X GET "$BASE_URL/api/time/active" \
  -H "Authorization: Bearer $TOKEN" | jq .
echo ""

echo "6. POST - Clock in WITH token"
curl -s -X POST "$BASE_URL/api/time/clockin" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "jobId": 1,
    "notes": "Starting work via authenticated API"
  }' | jq .
echo ""

echo "7. GET - Check active status (should be clocked in)"
curl -s -X GET "$BASE_URL/api/time/active" \
  -H "Authorization: Bearer $TOKEN" | jq .
echo ""

sleep 1

echo "8. POST - Clock out WITH token"
curl -s -X POST "$BASE_URL/api/time/clockout" \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "notes": "Done for now"
  }' | jq .
echo ""

echo "9. GET - View time history"
curl -s -X GET "$BASE_URL/api/time/history" \
  -H "Authorization: Bearer $TOKEN" | jq .
echo ""

# --- TEST USER ISOLATION ---

echo "10. POST - Set password for Jane Doe (ID 2)"
curl -s -X POST "$BASE_URL/api/auth/set-password" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 2,
    "password": "janepass"
  }' | jq .
echo ""

echo "11. POST - Login as Jane"
JANE_RESPONSE=$(curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "jane.doe@example.com",
    "password": "janepass"
  }')
echo "$JANE_RESPONSE" | jq .
JANE_TOKEN=$(echo "$JANE_RESPONSE" | jq -r '.token')
echo ""

echo "12. GET - Jane's history (should be separate from John's)"
curl -s -X GET "$BASE_URL/api/time/history" \
  -H "Authorization: Bearer $JANE_TOKEN" | jq .
echo ""

# --- VERIFY OTHER ENDPOINTS STILL WORK ---

echo "13. GET - Public endpoints still work (no auth needed)"
curl -s "$BASE_URL/api/status" | jq .
echo ""

echo "14. GET - Crew members endpoint (currently no auth required)"
curl -s "$BASE_URL/api/crewmembers" | jq '[.[] | {id, name: "\(.firstName) \(.lastName)", role}]'
echo ""

echo "=========================================="
echo "Authentication Tests Complete!"
echo "=========================================="
