#!/bin/bash
sudo unbound-control stats > $(pwd)/unbound-control-stats.txt
echo "domains.on.blocklist="$(cat /etc/unbound/unbound_blocklist.conf | wc -l) >> $(pwd)/unbound-control-stats.txt
