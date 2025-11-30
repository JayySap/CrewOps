#!/bin/bash

# CrewOps Time Tracking Test Script
# Run with: chmod +x test-time-tracking.sh && ./test-time-tracking.sh

BASE_URL="http://localhost:5152"

echo "=========================================="
echo "CrewOps Time Tracking Test Suite"
echo "=========================================="
echo ""

# --- CHECK EXISTING DATA ---

echo "1. GET existing crew members"
curl -s "$BASE_URL/api/crewmembers" | jq '[.[] | {id, name: "\(.firstName) \(.lastName)"}]'
echo ""

echo "2. GET existing jobs"
curl -s "$BASE_URL/api/jobs" | jq '[.[] | {id, referenceNumber, description}]'
echo ""

# --- CLOCK IN TESTS ---

echo "3. POST - Clock in John Smith (ID 1) to Job 1"
curl -s -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "jobId": 1,
    "notes": "Starting solar panel installation"
  }' | jq .
echo ""

echo "4. GET - Check if John is currently clocked in"
curl -s "$BASE_URL/api/time/active/1" | jq .
echo ""

echo "5. POST - Try to clock in John again (should fail with 400)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "jobId": 2,
    "notes": "Trying another job"
  }'
echo ""

echo "6. POST - Clock in Jane Doe (ID 2) to Job 1 (should work - different person)"
curl -s -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 2,
    "jobId": 1,
    "notes": "Joining the team"
  }' | jq .
echo ""

# --- CLOCK OUT TESTS ---

echo "Waiting 2 seconds to simulate work time..."
sleep 2
echo ""

echo "7. POST - Clock out John Smith"
curl -s -X POST "$BASE_URL/api/time/clockout" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "notes": "Completed first phase"
  }' | jq .
echo ""

echo "8. GET - Check if John is still clocked in (should be false)"
curl -s "$BASE_URL/api/time/active/1" | jq .
echo ""

echo "9. POST - Try to clock out John again (should fail - no active shift)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/time/clockout" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "notes": "Trying again"
  }'
echo ""

# --- SECOND SHIFT ---

echo "10. POST - Clock in John for a second shift on Job 2"
curl -s -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "jobId": 2,
    "notes": "HVAC maintenance work"
  }' | jq .
echo ""

sleep 1

echo "11. POST - Clock out John from second shift"
curl -s -X POST "$BASE_URL/api/time/clockout" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "notes": "Done for today"
  }' | jq .
echo ""

# --- CLOCK OUT JANE ---

echo "12. POST - Clock out Jane"
curl -s -X POST "$BASE_URL/api/time/clockout" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 2,
    "notes": "Finished assisting"
  }' | jq .
echo ""

# --- HISTORY ---

echo "13. GET - John's time history (should have 2 entries)"
curl -s "$BASE_URL/api/time/history/1" | jq .
echo ""

echo "14. GET - Jane's time history (should have 1 entry)"
curl -s "$BASE_URL/api/time/history/2" | jq .
echo ""

# --- ERROR CASES ---

echo "15. POST - Clock in non-existent crew member (ID 999)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 999,
    "jobId": 1,
    "notes": "test"
  }'
echo ""

echo "16. POST - Clock in to non-existent job (ID 999)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/time/clockin" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "jobId": 999,
    "notes": "test"
  }'
echo ""

echo "17. GET - History for non-existent crew member (ID 999)"
curl -s -w "\nHTTP Status: %{http_code}\n" "$BASE_URL/api/time/history/999"
echo ""

echo "=========================================="
echo "Time Tracking Tests Complete!"
echo "=========================================="
