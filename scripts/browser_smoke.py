"""Requires Playwright and Chromium. Start both local services before running."""
import os
from playwright.sync_api import sync_playwright
with sync_playwright() as p:
    executable=os.environ.get('CHROMIUM_PATH')
    browser=p.chromium.launch(headless=True,**({'executable_path':executable} if executable else {}))
    page=browser.new_page()
    errors=[]
    page.on('pageerror',lambda e: errors.append(str(e)))
    page.goto('http://127.0.0.1:5081')
    page.get_by_role('button',name='Valider et calibrer le snapshot').click()
    page.get_by_text('Snapshot SHA-256',exact=False).wait_for()
    page.get_by_role('button',name='Calibrer sur le ticket calculé').click()
    page.get_by_role('status').wait_for()
    page.get_by_role('button',name='Expliquer',exact=True).click()
    page.get_by_text('P&L exact',exact=True).wait_for()
    page.get_by_role('button',name='Simuler 20 jours').click()
    page.get_by_text('Erreur de couverture finale',exact=False).wait_for()
    page.get_by_label('Spot',exact=True).fill('-1')
    page.get_by_role('button',name='Calculer et actualiser les analyses').click()
    page.get_by_role('alert').wait_for()
    assert not errors,errors
    print('5 parcours Blazor interactifs réussis, sans erreur JavaScript.')
    browser.close()
