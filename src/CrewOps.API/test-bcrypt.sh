#!/bin/bash

# CrewOps BCrypt Password Hashing Test Script
# Run with: chmod +x test-bcrypt.sh && ./test-bcrypt.sh

BASE_URL="http://localhost:5152"

echo "=========================================="
echo "CrewOps BCrypt Password Hashing Test Suite"
echo "=========================================="
echo ""

# --- TEST EXISTING USERS (need new passwords since hashes changed) ---

echo "1. POST - Set hashed password for John Smith (ID 1)"
curl -s -X POST "$BASE_URL/api/auth/set-password" \
  -H "Content-Type: application/json" \
  -d '{
    "crewMemberId": 1,
    "password": "JohnSecure123!"
  }' | jq .
echo ""

echo "2. POST - Login with OLD plain password (should FAIL)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.smith@example.com",
    "password": "password123"
  }'
echo ""

echo "3. POST - Login with NEW hashed password (should SUCCEED)"
curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john.smith@example.com",
    "password": "JohnSecure123!"
  }' | jq .
echo ""

# --- TEST NEW USER CREATION WITH HASHING ---

echo "4. POST - Create new crew member with password"
curl -s -X POST "$BASE_URL/api/crewmembers" \
  -H "Content-Type: application/json" \
  -d '{
    "firstName": "Bob",
    "lastName": "Builder",
    "email": "bob.builder@example.com",
    "password": "BuildIt2024!",
    "role": "CrewMember"
  }' | jq .
echo ""

echo "5. POST - Login as Bob with correct password"
curl -s -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "bob.builder@example.com",
    "password": "BuildIt2024!"
  }' | jq .
echo ""

echo "6. POST - Login as Bob with wrong password (should FAIL)"
curl -s -w "\nHTTP Status: %{http_code}\n" -X POST "$BASE_URL/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "bob.builder@example.com",
    "password": "WrongPassword"
  }'
echo ""

# --- VERIFY PASSWORD NOT EXPOSED ---

echo "7. GET - Fetch crew members (passwordHash should NOT be visible... or check it's hashed)"
curl -s "$BASE_URL/api/crewmembers" | jq '.[] | {id, name: "\(.firstName) \(.lastName)", email, passwordHash: (.passwordHash // "NOT_SHOWN")}'
echo ""

echo "=========================================="
echo "BCrypt Tests Complete!"
echo "=========================================="
echo ""
echo "NOTE: The seeded admin user (admin@crewops.com) only works on a fresh database."
echo "      Your existing database already has users, so admin was not created."
