Feature: Get requested sessions
  Scenario: Get requested sessions when no session returns empty list
    Given user has right ApproveSession
    When GET to sessions?status=Requested
    And authenticated as a user
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

  Scenario: Get requested sessions when some sessions returns list
    Given user has right ApproveSession
    And a session entitled Session 1 with status Requested
    And a session entitled Session 2 with status Requested
    And a session entitled Session 3 with status Requested
    And a session entitled Session 4 with status Requested
    When GET to sessions?status=Requested
    And authenticated as a user
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

  Scenario: Get requested sessions when no right returns empty list
    When GET to sessions?status=Requested
    And authenticated as a user
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