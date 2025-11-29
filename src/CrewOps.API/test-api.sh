#!/bin/bash

# CrewOps API Test Script
# Run with: chmod +x test-api.sh && ./test-api.sh

BASE_URL="http://localhost:5152"

echo "=========================================="
echo "CrewOps API Test Suite"
echo "=========================================="
echo ""

# --- CREW MEMBERS ---

echo "1. GET all crew members"
curl -s "$BASE_URL/api/crewmembers" | jq .
echo ""

echo "2. POST - Create a new crew member (Jane Doe)"
curl -s -X POST "$BASE_URL/api/crewmembers" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Jane",
    "lastName": "Doe",
    "email": "jane.doe@example.com",
    "status": "Active"
  }' | jq .
echo ""

echo "3. GET all crew members (should now have 2)"
curl -s "$BASE_URL/api/crewmembers" | jq .
echo ""

# --- JOBS ---

echo "4. POST - Create Job 1 (Solar Panel Installation)"
curl -s -X POST "$BASE_URL/api/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "referenceNumber": "JOB-2024-001",
    "description": "Install solar panels at downtown office",
    "location": "123 Main Street, Downtown",
    "startDate": "2024-12-01T09:00:00"
  }' | jq .
echo ""

echo "5. POST - Create Job 2 (HVAC Maintenance)"
curl -s -X POST "$BASE_URL/api/jobs" \
  -H "Content-Type: application/json" \
  -d '{
    "referenceNumber": "JOB-2024-002",
    "description": "HVAC system maintenance",
    "location": "456 Oak Avenue, Uptown",
    "startDate": "2024-12-05T08:00:00"
  }' | jq .
echo ""

echo "6. GET all jobs"
curl -s "$BASE_URL/api/jobs" | jq .
echo ""

# --- ASSIGNMENTS ---

echo "7. POST - Assign John Smith (ID 1) to Job 1 as Lead"
curl -s -X POST "$BASE_URL/api/jobs/1/assign" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "role": "Lead"
  }' | jq .
echo ""

echo "8. POST - Assign Jane Doe (ID 2) to Job 1 as Member"
curl -s -X POST "$BASE_URL/api/jobs/1/assign" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 2,
    "role": "Member"
  }' | jq .
echo ""

echo "9. GET - View crew assigned to Job 1"
curl -s "$BASE_URL/api/jobs/1/crew" | jq .
echo ""

echo "10. POST - Try to assign John Smith to Job 1 again (should return 409 Conflict)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/jobs/1/assign" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "role": "Lead"
  }'
echo ""

echo "11. POST - Assign John Smith to Job 2 as well (should work - different job)"
curl -s -X POST "$BASE_URL/api/jobs/2/assign" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "role": "Lead"
  }' | jq .
echo ""

echo "12. GET - View crew assigned to Job 2"
curl -s "$BASE_URL/api/jobs/2/crew" | jq .
echo ""

# --- ERROR CASES ---

echo "13. POST - Try to assign non-existent crew member (ID 999)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/jobs/1/assign" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 999,
    "role": "Member"
  }'
echo ""

echo "14. GET - Try to get crew for non-existent job (ID 999)"
curl -s -w "\nHTTP Status: %{http_code}\n" "$BASE_URL/api/jobs/999/crew"
echo ""

echo "=========================================="
echo "Tests Complete!"
echo "=========================================="
