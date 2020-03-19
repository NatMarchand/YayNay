Feature: Get scheduled sessions
  Scenario: Get scheduled sessions when no session returns empty list
    When GET to sessions?status=Scheduled
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

  Scenario: Get scheduled sessions when some sessions returns list
    Given a session entitled Session 1 with status Scheduled
    And a session entitled Session 2 with status Scheduled
    And a session entitled Session 3 with status Scheduled
    And a session entitled Session 4 with status Scheduled
    When GET to sessions?status=Scheduled
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
      "status": "Scheduled",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 2",
      "description": "",
      "schedule": null,
      "status": "Scheduled",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 3",
      "description": "",
      "schedule": null,
      "status": "Scheduled",
      "tags": [],
      "speakers": []
    },
    {
      "id": "*",
      "title": "Session 4",
      "description": "",
      "schedule": null,
      "status": "Scheduled",
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