Feature: Get requested sessions
  Scenario: Get requested sessions when admin and no session returns empty list
    Given an admin
    When GET to sessions?status=Requested
    And header Authorization is Bearer admintoken
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": {
    "currentPage": 0,
    "totalPages": 1
  }
}
"""

  Scenario: Get requested sessions when admin and some sessions returns list
    Given an admin
    And a session entitled Session 1
    And a session entitled Session 2
    And a session entitled Session 3
    And a session entitled Session 4
    When GET to sessions?status=Requested
    And header Authorization is Bearer admintoken
    Then status code is OK
    And content matches
"""
{
  "values": [
    {
      "id": "*",
      "title": "Session 1",
      "description": "",
      "schedule": null,
      "status": "Requested",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 2",
      "description": "",
      "schedule": null,
      "status": "Requested",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 3",
      "description": "",
      "schedule": null,
      "status": "Requested",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 4",
      "description": "",
      "schedule": null,
      "status": "Requested",
      "tags": [],
      "speakers": []
    }
  ],
  "paging": {
    "currentPage": 0,
    "totalPages": 1
  }
}
"""

  Scenario: Get requested sessions when user returns empty list
    Given a user
    When GET to sessions?status=Requested
    And header Authorization is Bearer usertoken
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": null
}
"""

  Scenario: Get requested sessions when not authenticated returns empty list
    When GET to sessions?status=Requested
    Then status code is OK
    And content matches
"""
{
  "values": [],
  "paging": null
}
"""